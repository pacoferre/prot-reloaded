using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PROTR.Core.Lists
{
    public class ListProvider
    {
        private ConcurrentDictionary<Tuple<string, string, string>, Lazy<Task<ListTable>>>
            listProviders = new ConcurrentDictionary<Tuple<string, string, string>, Lazy<Task<ListTable>>>();

        public async Task<ListTable> GetList(ContextProvider contextProvider, string objectName, string listName = "", string parameter = "")
        {
            if (listName == "")
            {
                listName = objectName;
            }

            Lazy<Task<ListTable>> lazy = listProviders.GetOrAdd(
                new Tuple<string, string, string>(objectName, listName, parameter),
                new Lazy<Task<ListTable>>(
                    async () => await GetListInternal(contextProvider, objectName, listName, parameter),
                    LazyThreadSafetyMode.ExecutionAndPublication
                ));

            return await lazy.Value;
        }

        public async Task Invalidate(ContextProvider contextProvider, string objectName)
        {
            if (await contextProvider.BusinessProvider.ExistsData(Key(objectName, "", "")))
            {
                (await GetListInternal(contextProvider, objectName, "", "")).Invalidate();
            }

            foreach(var kp in listProviders)
            {
                if (kp.Key.Item1 == objectName)
                {
                    if (kp.Value.IsValueCreated)
                    {
                        (await kp.Value.Value).Invalidate();
                    }
                }
            }
        }

        private string Key(string objectName, string listName, string parameter)
        {
            return "list_" + objectName + "_" + listName + "_" + parameter;
        }

        private async Task<ListTable> GetListInternal(ContextProvider contextProvider, string objectName, string listName, string parameter)
        {
            string key = Key(objectName, listName, parameter);
            ListTable list;
            byte[] listData = await contextProvider.BusinessProvider.GetData(key);

            if (listData == null)
            {
                BusinessBaseDecorator def = contextProvider.BusinessProvider.GetDecorator(contextProvider, objectName);

                list = def.GetList(contextProvider, listName, parameter);

                await contextProvider.BusinessProvider.StoreData(key, list.Serialize());
            }
            else
            {
                list = new ListTable(contextProvider, listName, listData);
            }

            return list;
        }
    }
}
