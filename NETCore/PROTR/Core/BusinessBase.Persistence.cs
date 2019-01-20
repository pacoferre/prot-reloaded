using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core
{
    public partial class BusinessBase
    {
        public virtual async Task SetNew(bool preserve = false, bool withoutCollections = false)
        {
            if (!preserve)
            {
                dataItem = Decorator.New(this);
            }
            IsNew = true;

            if (!withoutCollections)
            {
                foreach (BusinessCollectionBase col in relatedCollections.Values)
                {
                    await col.SetNew(preserve, withoutCollections);
                }
            }
            PostSetNew();
        }

        public virtual async Task ReadFromDB(string key)
        {
            if (key.StartsWith("-"))
            {
                throw new Exception("Invalid key (" + key + ") for object " + ObjectName
                    + ". Minus char is for new object");
            }

            string[] keys = DataItem.SplitKey(key);

            if (keys.Length != Decorator.PrimaryKeys.Count)
            {
                throw new Exception("Invalid key (" + key + ") for object " + ObjectName);
            }

            foreach (int index in Decorator.PrimaryKeys)
            {
                await Decorator.ListProperties[index].SetValue(this, keys[index]);
            }

            await ReadFromDB();
        }

        public virtual async Task ReadFromDB(int key)
        {
            if (!Decorator.PrimaryKeyIsOneInt)
            {
                throw new Exception("Primary key is not int.");
            }

            this[Decorator.PrimaryKeys[0]] = key;

            await ReadFromDB();
        }

        public virtual async Task ReadFromDB(long key)
        {
            if (!Decorator.PrimaryKeyIsOneLong)
            {
                throw new Exception("Primary key is not long.");
            }

            this[Decorator.PrimaryKeys[0]] = key;

            await ReadFromDB();
        }

        public virtual async Task ReadFromDB(Guid key)
        {
            if (!Decorator.PrimaryKeyIsOneGuid)
            {
                throw new Exception("Primary key is not guid.");
            }

            this[Decorator.PrimaryKeys[0]] = key;

            await ReadFromDB();
        }

        public virtual async Task ReadFromDB()
        {
            if (!IsNew)
            {
                await contextProvider.DbContext.ReadBusinessObject(this);

                IsNew = false;
                IsModified = false;
                IsDeleting = false;

                AfterReadFromDB();
            }
        }

        protected virtual void AfterReadFromDB()
        {
            foreach (BusinessCollectionBase c in relatedCollections.Values)
            {
                c.Reset();
            }
        }

        public virtual async Task StoreToDB()
        {
            LastErrorMessage = "";
            LastErrorProperty = "";

            if (IsDeleting || IsNew || IsModified)
            {
                bool isValidated = IsDeleting ? true : Validate();

                if (isValidated)
                {
                    if (BeforeStoreToDB())
                    {
                        bool wasNew = IsNew;
                        bool wasModified = IsModified;
                        bool wasDeleting = IsDeleting;

                        if (IsDeleting)
                        {
                            foreach (BusinessCollectionBase col in relatedCollections.Values)
                            {
                                await col.SetForDeletion();
                                await col.StoreToDB();
                            }
                        }

                        await contextProvider.DbContext.StoreBusinessObject(this);

                        foreach (BusinessCollectionBase col in relatedCollections.Values)
                        {
                            if (col.MustSave)
                            {
                                await col.StoreToDB();
                            }
                        }

                        IsNew = false;
                        IsModified = false;
                        IsDeleting = false;

                        await businessProvider.ListProvider.Invalidate(contextProvider, ObjectName);

                        await AfterStoreToDB(wasNew, wasModified, wasDeleting);
                    }
                }
                else
                {
                    throw new Exception(LastErrorMessage);
                }
            }
        }

        protected virtual bool BeforeStoreToDB()
        {
            return true;
        }

        protected virtual Task AfterStoreToDB(bool wasNew, bool wasModified, bool wasDeleting)
        {
            return Task.CompletedTask;
        }

        public virtual void SetPropertiesFrom(BusinessBase source)
        {
        }

        public virtual void PostSetNew()
        {
        }

        public virtual void CopyTo(BusinessBase Target, List<string> excludeFieldNames)
        {
            foreach (PropertyDefinition prop in Decorator.ListProperties)
            {
                if (!prop.IsReadOnly
                    && !prop.IsPrimaryKey
                    && (excludeFieldNames == null || (excludeFieldNames != null
                    && !excludeFieldNames.Contains(prop.FieldName))))
                {
                    Target[prop.FieldName] = this[prop.FieldName];
                }
            }
        }
    }
}
