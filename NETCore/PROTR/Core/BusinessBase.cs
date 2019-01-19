using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PROTR.Core.Security;
using System.Data;

namespace PROTR.Core
{
    public partial class BusinessBase
    {
        protected BusinessBaseDecorator decorator = null;
        protected DataItem dataItem = null;
        protected BusinessBaseProvider businessProvider;
        protected ContextProvider contextProvider;

        public Type ModelType { get; set; }

        public BusinessBase(BusinessBaseProvider businessProvider, bool noDB)
        {
            this.businessProvider = businessProvider;
            decorator = businessProvider.GetDecorator(null, this.ToString().Split('.').Last(), 0);
            dataItem = Decorator.New(this);
        }

        public BusinessBase(ContextProvider contextProvider, string objectName = "", int dbNumber = 0)
        {
            if (objectName == "")
            {
                objectName = this.ToString().Split('.').Last();
            }

            this.businessProvider = contextProvider.BusinessProvider;
            this.contextProvider = contextProvider;
            decorator = businessProvider.GetDecorator(contextProvider, objectName, dbNumber);
            dataItem = Decorator.New(this);
        }

        public BusinessBaseDecorator Decorator => decorator;
        public BusinessBaseProvider BusinessProvider => businessProvider;
        public ContextProvider ContextProvider => contextProvider;

        public virtual string ObjectName
        {
            get
            {
                string objectName = ToString();

                if (objectName.Contains("BusinessBase"))
                {
                    return Decorator.ObjectName;
                }

                return objectName.Substring(objectName.LastIndexOf(".") + 1);
            }
        }

        public virtual string GenerateKey(object[] dataItemValues)
        {
            string key = "";

            foreach (int index in Decorator.PrimaryKeys)
            {
                key += dataItemValues[index].NoNullString() + "_";
            }

            return key.TrimEnd('_');
        }

        public string Key
        {
            get
            {
                return dataItem.Key;
            }
        }

        public object[] Keys
        {
            get {
                return Decorator
                    .PrimaryKeys
                    .Select(index => dataItem[index])
                    .ToArray();
            }
        }

        public virtual object this[string property]
        {
            get
            {
                return dataItem[Decorator.IndexOfName(property)];
            }
            set
            {
                dataItem[Decorator.IndexOfName(property)] = value;
            }
        }

        public virtual object this[int index]
        {
            get
            {
                return dataItem[index];
            }
            set
            {
                dataItem[index] = value;
            }
        }

        private AppUser _currentUser = null;
        public AppUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = contextProvider.GetAppUser();
                }

                return _currentUser;
            }
        }

        public override bool Equals(object obj)
        {
            // Stackoverflow... ups
            //if (obj is BusinessBase)
            //{
            //    return (BusinessBase)obj == this;
            //}

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (ObjectName + "_" + Key).GetHashCode();
        }

        public static bool operator ==(BusinessBase b1, BusinessBase b2)
        {
            if ((object)b1 == null && (object)b2 == null)
            {
                return true;
            }
            if ((object)b1 == null || (object)b2 == null)
            {
                return false;
            }
            if (b1.Key != b2.Key)
            {
                return false;
            }
            return true;
        }

        public static bool operator !=(BusinessBase b1, BusinessBase b2)
        {
            return !(b1 == b2);
        }
    }
}
