using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PROTR.Core
{
    public class BusinessBaseModel<T> : BusinessBase where T : class, new()
    {
        private Type _type;
        private PropertyInfo[] _properties;
        private Dictionary<string, PropertyInfo> _propertiesDict;

        public BusinessBaseModel(string objectName = "", int dbNumber = 0) : base(objectName, dbNumber)
        {
            CreateInstance();
        }

        public BusinessBaseModel(bool noDB) : base(noDB)
        {
            CreateInstance();
        }

        private void CreateInstance()
        {
            D = new T();
            _type = typeof(T);
            _properties = typeof(T).GetProperties();
            _propertiesDict = _properties.ToDictionary(prop => prop.Name.ToLower());
        }

        public T D { get; private set; }

        public override void StoreToDB()
        {
            base.StoreToDB();
        }

        public override object this[string property]
        {
            set
            {
                base[property] = value;

                SetValue(property, value);
            }
        }

        public override void PostSetNew()
        {
            base.PostSetNew();
            SetValues();
        }

        protected override void AfterReadFromDB()
        {
            base.AfterReadFromDB();
            SetValues();
        }

        private void GetValuesFromModel()
        {
            foreach (var prop in Decorator.ListProperties)
            {
                SetValueFromModel(prop);
            }
        }

        private void SetValues()
        {
            foreach(var prop in Decorator.ListProperties)
            {
                SetValue(prop.FieldName, this[prop.Index]);
            }
        }

        private void SetValue(string name, object value)
        {
            if (_propertiesDict.TryGetValue(name.ToLower(), out var property))
            {
                Type tPropertyType = property.PropertyType;
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                object newA = Convert.ChangeType(value, newT);
                property.SetValue(D, newA, null);
            }
        }

        private void SetValueFromModel(PropertyDefinition propDef)
        {
            if (_propertiesDict.TryGetValue(propDef.FieldName.ToLower(), out var property))
            {
                this[propDef.Index] = property.GetValue(D);
            }
        }
    }
}
