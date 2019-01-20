using PROTR.Core.Lists;
using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PROTR.Core
{
    public partial class BusinessBaseDecorator
    {
        public Dictionary<string, PropertyDefinition> Properties { get; } = new Dictionary<string, PropertyDefinition>();
        public List<PropertyDefinition> ListProperties { get; } = new List<PropertyDefinition>();
        public List<int> PrimaryKeys { get; } = new List<int>();
        public bool PrimaryKeyIsOneInt { get; internal set; }
        public bool PrimaryKeyIsOneLong { get; internal set; }
        public bool PrimaryKeyIsOneGuid { get; internal set; }
        public string PrimaryKeyFieldName { get; internal set; } = null;

        public string Singular { get; set; } = "";
        public string Plural { get; set; } = "";
        public string AllListDescription { get; set; } = "All";

        public bool[] setModified;
        private Dictionary<string, int> fieldNameLookup;
        protected int dbNumber;
        protected string tableName;
        protected string tableNameEncapsulated;
        protected string objectName;
        protected string[] names;
        protected PropertyDefinition firstStringProperty;

        protected BusinessBaseProvider provider;

        public BusinessBaseDecorator(BusinessBaseProvider provider)
        {
            this.provider = provider;
        }

        public int DBNumber
        {
            get
            {
                return dbNumber;
            }
        }

        public string TableNameEncapsulated
        {
            get
            {
                return tableNameEncapsulated;
            }
        }

        public string ObjectName
        {
            get
            {
                return objectName;
            }
        }

        internal int IndexOfName(string name)
        {
            return (name != null &&
                fieldNameLookup.TryGetValue(name, out int result)) ? result : -1;
        }

        public virtual void SetProperties(ContextProvider contextProvider, string objectName, int dbNumber)
        {
            string tableName = provider.GetDBTableFor(objectName);

            this.dbNumber = dbNumber;
            this.objectName = objectName;
            this.tableName = tableName;

            foreach (ColumnDefinition column in contextProvider.DbContext.GetDefinitions(objectName))
            {
                PropertyDefinition def = new PropertyDefinition(column);

                Properties[column.ColumnName] = def;
            }

            if (Properties.Count == 0)
            {
                throw new Exception("No Schema found for " + tableName + ".");
            }

            Singular = "";
            foreach (char letter in objectName)
            {
                if (Singular != "" && letter.ToString().ToUpper() == letter.ToString())
                {
                    Singular += " ";
                }

                Singular += letter;
            }

            Plural = Singular + "s";

            SetCustomProperties();

            PostSetCustomProperties();
        }

        protected virtual void SetCustomProperties()
        {
        }

        protected void PostSetCustomProperties()
        { 
            ListProperties.AddRange(Properties.Values.ToList());
            fieldNameLookup = new Dictionary<string, int>(Properties.Count, StringComparer.Ordinal);

            setModified = new bool[ListProperties.Count];

            if (firstStringProperty == null)
            {
                firstStringProperty = ListProperties.Find(prop => prop.BasicType == BasicType.Text
                    && !prop.IsPrimaryKey);
            }

            names = Properties.Keys.ToArray();
            for (int i = 0; i < Properties.Count; i++)
            {
                PropertyDefinition prop = Properties.ElementAt(i).Value;

                fieldNameLookup[names[i]] = i;
                setModified[i] = prop.SetModified;

                if (prop.IsPrimaryKey)
                {
                    PrimaryKeys.Add(i);
                }

                prop.Index = i;

                if (prop.Type == PropertyInputType.select)
                {
                    prop.DefaultSearch = "0";
                }
            }

            PrimaryKeyIsOneInt = PrimaryKeys.Count == 1 && 
                ListProperties[PrimaryKeys[0]].DataType == typeof(Int32);
            PrimaryKeyIsOneLong = PrimaryKeys.Count == 1 && 
                ListProperties[PrimaryKeys[0]].DataType == typeof(Int64);
            PrimaryKeyIsOneGuid = PrimaryKeys.Count == 1 && 
                ListProperties[PrimaryKeys[0]].DataType == typeof(Guid);

            PrimaryKeyFieldName = PrimaryKeys.Count == 1 ?
                ListProperties[PrimaryKeys[0]].FieldName : null;
        }

        public virtual DataItem New(BusinessBase owner)
        {
            object[] values = new object[ListProperties.Count];

            foreach(PropertyDefinition prop in ListProperties)
            { 
                values[prop.Index] = prop.DefaultValue;
            }

            return new DataItem(owner, values);
        }

        public PropertyDefinition FirstStringProperty
        {
            get
            {
                return firstStringProperty;
            }
        }

        public DynamicParameters GetPrimaryKeyParameters(BusinessBase obj)
        {
            DynamicParameters dynParms = new DynamicParameters();

            foreach (int pos in PrimaryKeys)
            {
                dynParms.Add("@" + names[pos], obj[pos]);
            }

            return dynParms;
        }

        public virtual string GetListSQL(DbDialect dialect, string listName, string parameter)
        {
            return "Select " + dialect.Encapsulate(names[PrimaryKeys[0]]) + " As ID, "
                + dialect.Encapsulate(firstStringProperty.FieldName) + " From " + tableNameEncapsulated
                + " Order By " + dialect.Encapsulate(firstStringProperty.FieldName);
        }

        public virtual Dictionary<string, object> GetParameter(string parameter)
        {
            if (parameter == "" || parameter == "0")
            {
                return null;
            }

            return new Dictionary<string, object>() { { "id", parameter } };
        }

        public virtual ListTable GetList(ContextProvider contextProvider, string listName = "", string parameter = "")
        {
            string sql = GetListSQL(contextProvider.DbDialect, listName, parameter);

            return new ListTable(contextProvider, listName, sql, GetParameter(parameter), DBNumber, AllListDescription);
        }
    }
}
