using PROTR.Core;
using PROTR.Core.Lists;
using PROTR.Core.REST;
using PROTR.Core.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using PROTR.Web.Dtos;
using System.Linq;
using PROTR.Core.DataViews;
using System.Threading.Tasks;

namespace PROTR.Web.Controllers
{
    public class CRUDController : Controller
    {
        protected IMemoryCache memoryCache;
        protected ContextProvider contextProvider;

        public CRUDController(IMemoryCache memoryCache, ContextProvider contextProvider)
        {
            this.memoryCache = memoryCache;
            this.contextProvider = contextProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!contextProvider.UserIsAuthenticated)
            {
                context.Result = new JsonResult(ModelToClient.ErrorResponse("User not authenticated"));
            }
        }

        [HttpGet]
        public Dictionary<string, PropertyDefinitionDto> Properties(string name)
        {
            return contextProvider
                .BusinessProvider
                .GetDecorator(contextProvider, name)
                .ListProperties
                .ToDictionary(prop => prop.FieldName.ToCamelCase(), prop => new PropertyDefinitionDto(prop));
        }

        [HttpPost]
        public async Task<ListModelToClient> List([FromBody]ListModelFromClient request)
        {
            ListModelToClient resp;
            FilterBase filter = await contextProvider
                .BusinessProvider
                .GetFilter(contextProvider, request.objectName, request.filterName);

            resp = await filter.ProcessRequestAndCreateResponse(HttpContext, request);

            await contextProvider
                .BusinessProvider
                .StoreFilter(contextProvider, filter, request.objectName, request.filterName);

            return resp;
        }

        [HttpPost]
        public async Task<List<DataViewColumn>> ListDefinition([FromBody]ListModelDefinitionRequest request)
        {
            FilterBase filter = await contextProvider
                .BusinessProvider
                .GetFilter(contextProvider, request.objectName, request.filterName);

            return new DataView(filter).Columns;
        }


        [HttpPost]
        public async Task<List<ListItemRest>> SimpleList([FromBody]SimpleListRequest request)
        {
            ListTable table = await contextProvider
                .BusinessProvider
                .ListProvider.GetList(contextProvider, request.objectName,
                    request.listName, request.parameter);

            return table.ToClient;
        }

        [HttpPost]
        public async Task<JsonResult> Post([FromBody]ModelFromClient fromClient)
        {
            fromClient.Sanitize();

            try
            {
                if (fromClient.action == "init")
                {
                    ModelToClient toClient = new ModelToClient
                    {
                        formToken = Guid.NewGuid().ToString(),
                        sequence = 1,
                        action = "init"
                    };

                    return new JsonResult(toClient);
                }
                else if (fromClient.action == "changed" || fromClient.action == "load"
                    || fromClient.action == "ok" || fromClient.action == "clear"
                    || fromClient.action == "new" || fromClient.action == "delete")
                {
                    if (fromClient.action == "new")
                    {
                        fromClient.root.key = "0";
                    }

                    return new JsonResult(await (await contextProvider.BusinessProvider.RetreiveObject(contextProvider, fromClient.objectName,
                        fromClient.root.key)).PerformActionAndCreateResponse(HttpContext, fromClient));
                }

                return new JsonResult(ModelToClient.ErrorResponse("Action " + fromClient.action + " not supported."));
            }
            catch (Exception exp)
            {
                return new JsonResult(ModelToClient.ErrorResponse(exp.Message));
            }
        }

        //private static Dictionary<Guid, string> cruds = new Dictionary<Guid, string>();

        //public object DapperRow { get; private set; }
    }
}
