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
            Func<ContextProvider, BusinessBase> creator;

            if (creators.TryGetValue(objectName, out creator))
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

        public FilterBase GetFilter(ContextProvider contextProvider, string objectName, string filterName = "")
        {
            AppUser user = contextProvider.GetAppUser();
            string filterKey = FilterKey(user, objectName, filterName);
            object objTemp;
            byte[] data;

            FilterBase filter = GetDecorator(contextProvider, objectName, 0).GetFilter(contextProvider, filterName);

            data = GetData(filterKey);

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

        public void StoreFilter(ContextProvider contextProvider, FilterBase filter, string objectName, string filterName)
        {
            AppUser user = contextProvider.GetAppUser();
            string filterKey = FilterKey(user, objectName, filterName);
            StoreData(filterKey, filter.Serialize());
        }

        private string FilterKey(AppUser user, string objectName, string filterName)
        {
            return "F_" + objectName + "_" + filterName + "_" + user.Key;
        }

        private string ObjectKey(string objectName, int dbNumber, string key)
        {
            return "O_" + objectName + "_" + dbNumber + "_" + key;
        }

        public void StoreObject(BusinessBase obj, string objectName)
        {
            string objectKey = ObjectKey(objectName, obj.Decorator.DBNumber, obj.Key);

            StoreData(objectKey, obj.Serialize());
        }

        public void StoreData(string key, byte[] data)
        {
            TheCache.GetDatabase().StringSetAsync(key, data, TimeSpan.FromMinutes(30), When.Always, CommandFlags.FireAndForget);
        }

        public bool ExistsData(string key)
        {
            return TheCache.GetDatabase().KeyExists(key);
        }

        public byte[] GetData(string key)
        {
            return TheCache.GetDatabase().StringGet(key);
        }

        public void RemoveData(string key)
        {
            TheCache.GetDatabase().KeyDeleteAsync(key, CommandFlags.FireAndForget);
        }

        public BusinessBase RetreiveObject(ContextProvider contextProvider, string objectName, string key)
        {
            return RetreiveObject(contextProvider, objectName, 0, key);
        }

        public BusinessBase RetreiveObject(ContextProvider contextProvider, string objectName, int dbNumber, string key)
        {
            string objectKey = ObjectKey(objectName, dbNumber, key);
            BusinessBase objResp = contextProvider.BusinessItems.GetOrAdd(objectKey, (theKey) =>
                {
                    byte[] data;
                    bool readFromDB = true;
                    BusinessBase obj = CreateObject(contextProvider, objectName, dbNumber);

                    data = GetData(objectKey);

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
                            obj.ReadFromDB(key);
                        }
                        else
                        {
                            if (!obj.IsNew)
                            {
                                obj.SetNew();
                                objectKey = obj.Key;
                            }
                        }

                        StoreObject(obj, objectName);
                    }

                    return obj;
                });

            return objResp;
        }
    }
}
