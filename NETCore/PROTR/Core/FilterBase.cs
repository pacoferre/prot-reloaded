﻿using PROTR.Core.DataViews;
using PROTR.Core.REST;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace PROTR.Core
{
    public partial class FilterBase : IDataViewSetter
    {
        public bool EmptyWhereReturnsEmpty { get; set; } = false;
        public string FastSearch { get; set; } = "";
        public bool FastSearchActivated { get; set; } = true;
        public Dictionary<string, string> Filter { get; set; } = null;
        public int topRecords { get; set; } = 100;

        public BusinessBaseDecorator Decorator { get; }
        public ContextProvider ContextProvider { get; }

        public FilterBase(ContextProvider contextProvider, BusinessBaseDecorator decorator, int dbNumber = 0)
        {
            ContextProvider = contextProvider;
            Decorator = decorator;
        }

        public void Clear()
        {
            for (int index = 0; index < Filter.Count; ++index)
            {
                string key = Filter.Keys.ElementAt(index);
                PropertyDefinition prop;
                string fieldName = key;

                if (fieldName.Contains('|'))
                {
                    string[] parts = fieldName.Split('|');

                    fieldName = parts[0];
                }

                if (Decorator.Properties.TryGetValue(fieldName, out prop))
                {
                    Filter[key] = prop.DefaultSearch;
                }
            }
        }

        protected virtual Tuple<string, DynamicParameters> Where(DataView dataView)
        {
            string where = "";
            DynamicParameters param = new DynamicParameters();

            if (FastSearchActivated && FastSearch != "")
            {
                foreach(DataViewColumn col in dataView.VisibleColumns)
                {
                    if (where != "")
                    {
                        where += " OR ";
                    }

                    where += col.Expression + " LIKE @" + col.As;
                    param.Add(col.As, "%" + FastSearch + "%");
                }
            }
            else
            {
                foreach(KeyValuePair<string, string> item in Filter)
                {
                    PropertyDefinition prop;
                    string whereFieldName = item.Key;
                    string whereOperation = "";

                    if (whereFieldName.Contains('|'))
                    {
                        string[] parts = whereFieldName.Split('|');

                        whereFieldName = parts[0];
                        whereOperation = parts[1];
                    }
                    
                    if (Decorator.Properties.TryGetValue(whereFieldName, out prop))
                    {
                        prop.Where(ref where, ref param, item.Value, whereOperation);
                    }
                }

                where = where.Replace("[TABLENAME]", Decorator.TableNameEncapsulated);
            }

            return new Tuple<string, DynamicParameters>(where, param);
        }

        public static Tuple<string, DynamicParameters> emptyWhere 
            = new Tuple<string, DynamicParameters>("1 = 0", null);

        public virtual async Task<(IEnumerable<dynamic>, int)> Get(int order, SortDirection sortDirection,
            int pageNumber, int rowsPerPage)
        {
            DataView dataView = new DataView(this);
            Tuple<string, DynamicParameters> where = Where(dataView);

            if (where.Item1 == "" && EmptyWhereReturnsEmpty)
            {
                where = emptyWhere;
            }

            return await dataView.Get(where.Item1, where.Item2, order, sortDirection, pageNumber, rowsPerPage);
        }

        public DataView GetEmpty()
        {
            return new DataView(this);
        }

        public virtual void SetDataView(DataView dataView)
        {
            dataView.Columns = new List<DataViewColumn>(2);
            dataView.Columns.Add(new DataViewColumn(Decorator.TableNameEncapsulated,
                Decorator.ListProperties[0]));
            if (Decorator.FirstStringProperty != null)
            {
                dataView.Columns.Add(new DataViewColumn(Decorator.TableNameEncapsulated,
                    Decorator.FirstStringProperty));
            }

            dataView.FromClause = Decorator.TableNameEncapsulated;
        }

        public virtual string GetCountSQLQuery(DataView dataView, string whereClause, object param)
        {
            // {SelectColumns} {FromClause} {WhereClause} {OrderBy} {PageNumber} {RowsPerPage}
            string sql = dataView.query;
            string where = dataView.PreWhere;

            if (where == "")
            {
                where = whereClause;
            }
            else
            {
                where = "(" + where + ")" + (whereClause == "" ? "" : " AND " + whereClause);
            }

            sql = sql
                .Replace("{WhereClause}", (where == "" ? "" : "WHERE " + where));

            return sql;
        }

        public virtual string GetFinalSQLQuery(DataView dataView, string whereClause, object param, int order,
            SortDirection sortDirection, int pageNumber, int rowsPerPage)
        {
            // {SelectColumns} {FromClause} {WhereClause} {OrderBy} {PageNumber} {RowsPerPage}
            string sql = dataView.query;
            string orderBy = dataView.firstOrderBy;
            string where = dataView.PreWhere;

            if (dataView.visibleColumns[order].OrderBy != "" && !dataView.visibleColumns[order].Hidden)
            {
                orderBy = dataView.visibleColumns[order].OrderBy;

                //if (visibleColumns.Count > order + 1)
                //{
                //    orderBy += "," + visibleColumns[order + 1].OrderBy;
                //}
            }

            if (dataView.PreOrderBy != "")
            {
                orderBy = dataView.PreOrderBy + (orderBy == "" ? "" : ", " + orderBy);
            }

            if (dataView.PostOrderBy != "" && !orderBy.Contains(dataView.PostOrderBy))
            {
                orderBy += (orderBy == "" ? "" : ", " + dataView.PostOrderBy);
            }

            if (sortDirection == SortDirection.Descending)
            {
                if (orderBy != "")
                {
                    orderBy = orderBy.Replace(",", " DESC ,");
                    orderBy += " DESC";
                }
            }

            if (where == "")
            {
                where = whereClause;
            }
            else
            {
                where = "(" + where + ")" + (whereClause == "" ? "" : " AND " + whereClause);
            }

            sql = sql.Replace("{OrderBy}", orderBy)
                .Replace("{WhereClause}", (where == "" ? "" : "WHERE " + where))
                .Replace("{PageNumber}", pageNumber.ToString())
                .Replace("{RowsPerPage}", rowsPerPage.ToString());

            return sql;
        }

        public virtual byte[] Serialize()
        {
            JObject obj = ToJObject();

            return Encoding.Unicode.GetBytes(obj.ToString(Newtonsoft.Json.Formatting.None));
        }

        public JObject ToJObject()
        {
            JObject obj = new JObject();

            obj.Add("t", JToken.FromObject(topRecords));
            obj.Add("f", JToken.FromObject(Filter));

            return obj;
        }

        public virtual void Deserialize(byte[] data)
        {
            string json = Encoding.Unicode.GetString(data);
            JObject obj = JObject.Parse(json);

            FromJObject(obj);
        }

        public void FromJObject(JObject obj)
        {
            topRecords = obj["t"].ToObject<int>();
            Filter = obj["f"].ToObject<Dictionary<string, string>>();
        }
    }
}
