using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROTR.Core.DataViews
{
    public interface IDataViewSetter
    {
        ContextProvider ContextProvider { get; }

        void SetDataView(DataView dataView);

        string GetFinalSQLQuery(DataView dataView, string whereClause, object param, int order, SortDirection sortDirection,
            int pageNumber, int rowsPerPage);

        string GetCountSQLQuery(DataView dataView, string whereClause, object param);
    }

    public class DataView
    {
        public List<DataViewColumn> Columns { get; set; }
        public string FromClause { get; set; } = "";
        public string GroupByClause { get; set; } = "";
        public string PreOrderBy { get; set; } = "";
        public string PreWhere { get; set; } = "";
        public string PostOrderBy { get; set; } = "";

        public List<DataViewColumn> visibleColumns;
        private string selectNamedColumns = "";
        private string selectColumns = "";
        public string query = "";
        public string firstOrderBy = "";
        protected IDataViewSetter setter;

        public DataView(IDataViewSetter setter)
        {
            this.setter = setter;
            this.setter.SetDataView(this);

            InternalSet();
        }

        public List<DataViewColumn> VisibleColumns
        {
            get
            {
                return visibleColumns;
            }
        }

        private void InternalSet()
        {
            int order = 0;
            int index = 0;

            visibleColumns = new List<DataViewColumn>(Columns.Count);

            foreach (DataViewColumn gc in Columns)
            {
                if (gc.IsID)
                {
                    gc.As = "ID";
                }
                else
                {
                    gc.As = "C" + index;
                }
                index++;

                if (gc.Visible)
                {
                    visibleColumns.Add(gc);

                    if (selectColumns != "")
                    {
                        selectColumns += ",";
                        selectNamedColumns += ",";
                    }
                    selectColumns += gc.Expression + " As " + gc.As;
                    selectNamedColumns += gc.As;

                    if (!gc.IsID && !gc.Hidden && firstOrderBy == "" && gc.OrderBy != "")
                    {
                        firstOrderBy = gc.OrderBy;
                    }

                    order++;
                }
            }

            if (FromClause == null)
            {
                throw new Exception("FromClause must be set.");
            }
            if (selectColumns == null)
            {
                throw new Exception("No columns to show.");
            }

            // {SelectColumns} {FromClause} {WhereClause} {OrderBy} {PageNumber} {RowsPerPage}
            query = setter.ContextProvider.DbDialect.GetPagedListSql
                .Replace("{SelectColumns}", selectColumns)
                .Replace("{SelectNamedColumns}", selectNamedColumns)
                .Replace("{GroupByClause}", (GroupByClause == "" ? "" : "GROUP BY " + GroupByClause))
                .Replace("{FromClause}", FromClause);
        }

        public IEnumerable<dynamic> Get(string whereClause, DynamicParameters param, int order, SortDirection sortDirection,
            int pageNumber, int rowsPerPage, ref int rowCount)
        {
            string sql = setter.GetFinalSQLQuery(this, whereClause, param, order, sortDirection, pageNumber, rowsPerPage);
            string sqlCount = setter.GetCountSQLQuery(this, whereClause, param);

            rowCount = setter.ContextProvider.DbContext.ParametrizedQuery<int>(sql, param).FirstOrDefault();

            return setter.ContextProvider.DbContext.ParametrizedQuery<dynamic>(sql, param);
        }
    }
}
