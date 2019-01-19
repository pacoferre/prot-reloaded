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
        private ConcurrentDictionary<Tuple<string, string, string>, Lazy<ListTable>>
            listProviders = new ConcurrentDictionary<Tuple<string, string, string>, Lazy<ListTable>>();
        private ContextProvider contextProvider;

        public ListTable GetList(ContextProvider contextProvider, string objectName, string listName = "", string parameter = "")
        {
            if (listName == "")
            {
                listName = objectName;
            }
            this.contextProvider = contextProvider;

            Lazy<ListTable> lazy = listProviders.GetOrAdd(
                new Tuple<string, string, string>(objectName, listName, parameter),
                new Lazy<ListTable>(
                    () => GetListInternal(contextProvider, objectName, listName, parameter),
                    LazyThreadSafetyMode.ExecutionAndPublication
                ));

            return lazy.Value;
        }

        public void Invalidate(string objectName)
        {
            if (contextProvider.BusinessProvider.ExistsData(Key(objectName, "", "")))
            {
                GetListInternal(contextProvider, objectName, "", "").Invalidate();
            }

            foreach(var kp in listProviders)
            {
                if (kp.Key.Item1 == objectName)
                {
                    if (kp.Value.IsValueCreated)
                    {
                        kp.Value.Value.Invalidate();
                    }
                }
            }
        }

        private string Key(string objectName, string listName, string parameter)
        {
            return "list_" + objectName + "_" + listName + "_" + parameter;
        }

        private ListTable GetListInternal(ContextProvider contextProvider, string objectName, string listName, string parameter)
        {
            string key = Key(objectName, listName, parameter);
            ListTable list;
            byte[] listData = contextProvider.BusinessProvider.GetData(key);

            if (listData == null)
            {
                BusinessBaseDecorator def = contextProvider.BusinessProvider.GetDecorator(contextProvider, objectName);

                list = def.GetList(contextProvider, listName, parameter);

                contextProvider.BusinessProvider.StoreData(key, list.Serialize());
            }
            else
            {
                list = new ListTable(contextProvider, listName, listData);
            }

            return list;
        }
    }
}
