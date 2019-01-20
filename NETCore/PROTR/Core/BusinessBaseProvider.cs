using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;
using PROTR.Core.Security;
using Microsoft.Extensions.Configuration;
using System.Text;
using StackExchange.Redis;
using PROTR.Core.Lists;
using System.Threading.Tasks;

namespace PROTR.Core
{
    public class BusinessBaseProvider
    {
        public static Dictionary<string, string> TableSchemas { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> ObjectToDBTable { get; set; } = new Dictionary<string, string>();

        public Func<ContextProvider, string, int, BusinessBase> DefaultBusinessBase;
        public Func<BusinessBaseDecorator> DefaultBusinessBaseDecorator;

        protected Dictionary<string, Func<ContextProvider, BusinessBase>> creators = new Dictionary<string, Func<ContextProvider, BusinessBase>>();
        protected Dictionary<string, Func<BusinessBaseDecorator>> decorators = new Dictionary<string, Func<BusinessBaseDecorator>>();
        private ConcurrentDictionary<string, Lazy<BusinessBaseDecorator>>
            decoratorsCreators = new ConcurrentDictionary<string, Lazy<BusinessBaseDecorator>>();
        public ListProvider ListProvider { get; private set; }
        private ConnectionMultiplexer TheCache = null;

        public void Configure(IConfiguration configuration, ConnectionMultiplexer theCache)
        {
            ContextProvider.Configuration = configuration;

            AppUser.SALT = Encoding.ASCII.GetBytes(configuration.GetSection("Security")["SALT"]).Take(16).ToArray();

            ListProvider = new ListProvider();
            TheCache = theCache;

            DefaultBusinessBase = (contextProvider, objectName, dbNumber) => new BusinessBase(contextProvider, objectName, dbNumber);
            DefaultBusinessBaseDecorator = () => new BusinessBaseDecorator(this);
        }

        public string GetDBTableFor(string objectName)
        {
            if (ObjectToDBTable.Keys.Contains(objectName))
            {
                return ObjectToDBTable[objectName];
            }

            return objectName;
        }

        public virtual void RegisterBusinessCreators()
        {
            creators.Add("AppUser", (contextProvider) => new Security.AppUser(contextProvider));
            decorators.Add("AppUser", () => new Security.AppUserDecorator(this));

            creators.Add("AppUserNoDB", (contextProvider) => new Security.AppUserNoDB(this));
            decorators.Add("AppUserNoDB", () => new Security.AppUserNoDBDecorator(this));
        }

        public virtual BusinessBase CreateObject(ContextProvider contextProvider, string objectName, int dbNumber = 0)
        {
            BusinessBase obj;

            if (creators.TryGetValue(objectName, out Func<ContextProvider, BusinessBase> creator))
            {
                obj = creator.Invoke(contextProvider);

                if (dbNumber != 0)
                {
                    //obj.ChangeDBNumber(dbNumber);
                }
            }
            else
            {
                obj = DefaultBusinessBase(contextProvider, objectName, dbNumber);
            }

            return obj;
        }

        public bool IsDecoratorCreated(ContextProvider contextProvider, string name, int dbNumber = 0)
        {
            return GetLazyDecorator(contextProvider, name, dbNumber).IsValueCreated;
        }

        public BusinessBaseDecorator GetDecorator(ContextProvider contextProvider, string name, int dbNumber = 0)
        {
            return GetLazyDecorator(contextProvider, name, dbNumber).Value;
        }

        private Lazy<BusinessBaseDecorator> GetLazyDecorator(ContextProvider contextProvider, string name, int dbNumber = 0)
        {
            return decoratorsCreators.GetOrAdd(
                name,
                new Lazy<BusinessBaseDecorator>(
                    () => GetDecoratorInternal(contextProvider, name, dbNumber),
                    LazyThreadSafetyMode.ExecutionAndPublication
                ));
        }

        private BusinessBaseDecorator GetDecoratorInternal(ContextProvider contextProvider, string objectName, int dbNumber)
        {
            BusinessBaseDecorator decorator;

            if (decorators.ContainsKey(objectName))
            {
                decorator = decorators[objectName].Invoke();
            }
            else
            {
                decorator = DefaultBusinessBaseDecorator();
            }
            decorator.SetProperties(contextProvider, objectName, dbNumber);

            return decorator;
        }

        public async Task<FilterBase> GetFilter(ContextProvider contextProvider, string objectName, string filterName = "")
        {
            AppUser user = await contextProvider.GetAppUser();
            string filterKey = FilterKey(user, objectName, filterName);
            byte[] data;

            FilterBase filter = GetDecorator(contextProvider, objectName, 0)
                .GetFilter(contextProvider, filterName);

            data = await GetData(filterKey);

            if (data != null)
            {
                try
                {
                    filter.Deserialize(data);
                }
                catch
                {
                    // Sometimes Redis returns bad data.
                }
            }

            return filter;
        }

        public async Task StoreFilter(ContextProvider contextProvider, FilterBase filter, string objectName, string filterName)
        {
            AppUser user = await contextProvider.GetAppUser();
            string filterKey = FilterKey(user, objectName, filterName);
            await StoreData(filterKey, filter.Serialize());
        }

        private string FilterKey(AppUser user, string objectName, string filterName)
        {
            return "F_" + objectName + "_" + filterName + "_" + user.Key;
        }

        private string ObjectKey(string objectName, int dbNumber, string key)
        {
            return "O_" + objectName + "_" + dbNumber + "_" + key;
        }

        public async Task StoreObject(BusinessBase obj, string objectName)
        {
            string objectKey = ObjectKey(objectName, obj.Decorator.DBNumber, obj.Key);

            await StoreData(objectKey, obj.Serialize());
        }

        public async Task StoreData(string key, byte[] data)
        {
            await TheCache.GetDatabase().StringSetAsync(key, data, TimeSpan.FromMinutes(30), When.Always, CommandFlags.FireAndForget);
        }

        public async Task<bool> ExistsData(string key)
        {
            return await TheCache.GetDatabase().KeyExistsAsync(key);
        }

        public async Task<byte[]> GetData(string key)
        {
            return await TheCache.GetDatabase().StringGetAsync(key);
        }

        public async Task RemoveData(string key)
        {
            await TheCache.GetDatabase().KeyDeleteAsync(key, CommandFlags.FireAndForget);
        }

        public async Task<BusinessBase> RetreiveObject(ContextProvider contextProvider, string objectName, string key)
        {
            return await RetreiveObject(contextProvider, objectName, 0, key);
        }

        public async Task<BusinessBase> RetreiveObject(ContextProvider contextProvider, string objectName, int dbNumber, string key)
        {
            string objectKey = ObjectKey(objectName, dbNumber, key);
            BusinessBase objResp = await Task.Run(() =>
            {
                return contextProvider.BusinessItems.GetOrAdd(objectKey, (theKey) =>
                    Get(contextProvider, objectName, dbNumber, key, objectKey).Result);
            });

            return objResp;
        }

        private async Task<BusinessBase> Get(ContextProvider contextProvider, string objectName, int dbNumber, string key, string objectKey)
        {
            byte[] data;
            bool readFromDB = true;
            BusinessBase obj = CreateObject(contextProvider, objectName, dbNumber);

            data = await GetData(objectKey);

            if (data != null)
            {
                try
                {
                    obj.Deserialize(data);
                    readFromDB = false;
                }
                catch
                {
                    // Sometimes Redis returns bad data.
                }
            }
            if (readFromDB)
            {
                if (key != "0" && key[0] != '-')
                {
                    await obj.ReadFromDB(key);
                }
                else
                {
                    if (!obj.IsNew)
                    {
                        await obj.SetNew();
                        objectKey = obj.Key;
                    }
                }

                await StoreObject(obj, objectName);
            }

            return obj;
        }
    }
}
