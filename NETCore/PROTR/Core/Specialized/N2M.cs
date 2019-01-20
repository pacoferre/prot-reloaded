using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.Specialized
{
    public class N2MDecorator : BusinessBaseDecorator
    {
        public string ExternalFieldNameM { get; set; }

        public N2MDecorator(BusinessBaseProvider provider) : base(provider)
        {

        }

        protected override void SetCustomProperties()
        {
            PropertyDefinition external = new PropertyDefinition(ExternalFieldNameM, ExternalFieldNameM, typeof(Int32));
            PropertyDefinition active = new PropertyDefinition("Active", "Active", typeof(bool), PropertyInputType.checkbox);

            Properties.Add(ExternalFieldNameM, external);
            Properties.Add("Active", active);

            Properties.Values.ElementAt(0).NoChecking = true;
            Properties.Values.ElementAt(1).NoChecking = true;

            base.SetCustomProperties();
        }
    }

    public class N2M : BusinessBase
    {
        protected string externalFieldNameM = "";

        protected string[] opcCampoOtros;

        public N2M(ContextProvider contextProvider, string objectName) : base(contextProvider, objectName)
        {
        }

        public override string GenerateKey(object[] dataItemValues)
        {
            //if (Parent != null && Parent.Parent != null)
            //{
            //    return Parent.Parent.Key + "_" + this[externalFieldNameM].ToString();
            //}
            return this[externalFieldNameM].ToString();
        }

        public string ExternalFieldNameM
        {
            get
            {
                return externalFieldNameM;
            }
        }

        public override async Task StoreToDB()
        {
            string ownFieldNameN = Decorator.ListProperties[0].FieldName;
            string ownFieldNameM = Decorator.ListProperties[1].FieldName;
            int externalID = (int)base[externalFieldNameM] != 0 ? (int)base[externalFieldNameM]
                : (int)base[ownFieldNameM];
            bool active = (bool)this["Active"];
            string sqlExists = "Select count(*) From " + Decorator.TableNameEncapsulated
                + " WHERE " + Decorator.ListProperties[0].FieldName + " = " + Parent.Parent.Key
                + " AND " + Decorator.ListProperties[1].FieldName + " = " + externalID;
            bool exists = await contextProvider.DbContext.QueryFirstOrDefaultAsync<int>(sqlExists) != 0;

            if (Parent.Parent.IsDeleting || !active)
            {
                if (exists)
                {
                    base[ownFieldNameN] = Parent.Parent.Key.NoNullInt();
                    base[ownFieldNameM] = externalID;

                    if (!IsDeleting)
                    {
                        IsDeleting = true;
                    }

                    await base.StoreToDB();
                }
            }
            else
            {
                if (!exists && !IsNew)
                {
                    await SetNew();
                }

                base[ownFieldNameN] = Parent.Parent.Key.NoNullInt();
                base[ownFieldNameM] = externalID;

                await base.StoreToDB();
            }
        }

        public override async Task SetNew(bool preserve = false, bool withoutCollections = false)
        {
            await base.SetNew(true, true);
        }

        public override bool MatchFilter(string filterName)
        {
            return ((bool)this["Active"]);
        }

        public override bool ValidateDataItem(DataItem dataItem, ref string lastErrorMessage, ref string lastErrorProperty)
        {
            if ((bool)this["Active"])
            {
                return base.ValidateDataItem(dataItem, ref lastErrorMessage, ref lastErrorProperty);
            }

            return true;
        }
    }
}
