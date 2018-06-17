using PROTR.Core.REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core
{
    public partial class FilterBase
    {
        public virtual ListModelToClient ProcessRequestAndCreateResponse(HttpContext context, ListModelFromClient request)
        {
            ListModelToClient resp = new ListModelToClient();
            int rowCount = 0;

            this.FastSearchActivated = request.dofastsearch;
            this.FastSearch = request.fastsearch;
            if (!request.first)
            {
                this.Filter = request.filters;
                this.topRecords = request.topRecords;
            }

            if (this.Filter == null)
            {
                this.Filter = request.filters;
                this.Clear();
            }

            if (request.sortDir != "asc" && request.sortDir != "desc")
            {
                request.sortDir = "asc";
            }

            resp.plural = this.Decorator.Plural;
            resp.filters = this.Filter;
            resp.result = Dapper.SqlMapper.ToList(this.Get(request.sortIndex,
                (request.sortDir == "asc" ? SortDirection.Ascending : SortDirection.Descending),
                request.pageNumber, request.rowsPerPage, ref rowCount));
            resp.fastsearch = this.FastSearch;
            resp.sortIndex = request.sortIndex;
            resp.sortDir = request.sortDir;
            resp.topRecords = this.topRecords;
            resp.pageNumber = request.pageNumber;
            resp.rowsPerPage = request.rowsPerPage;
            resp.rowCount = rowCount;

            this.SetExtraToClientResponse(resp);

            return resp;
        }
        protected virtual void SetExtraToClientResponse(ListModelToClient model)
        {

        }
    }
}
