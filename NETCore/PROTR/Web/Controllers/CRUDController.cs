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

namespace PROTR.Web.Controllers
{
    public class CRUDController : Controller
    {
        protected IMemoryCache memoryCache;

        public CRUDController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!AppUser.UserIsAuthenticated(context.HttpContext))
            {
                context.Result = new JsonResult(ModelToClient.ErrorResponse("User not authenticated"));
            }
        }

        [HttpGet]
        public Dictionary<string, PropertyDefinitionDto> Properties(string name)
        {
            return BusinessBaseProvider
                .Instance
                .GetDecorator(name)
                .ListProperties
                .ToDictionary(prop => prop.FieldName, prop => new PropertyDefinitionDto(prop));
        }

        [HttpPost]
        public ListModelToClient List([FromBody]ListModelFromClient request)
        {
            ListModelToClient resp;
            FilterBase filter = BusinessBaseProvider.Instance.GetFilter(HttpContext, request.objectName, request.filterName);

            resp = filter.ProcessRequestAndCreateResponse(HttpContext, request);

            BusinessBaseProvider.StoreFilter(HttpContext, filter, request.objectName, request.filterName);

            return resp;
        }

        [HttpPost]
        public List<DataViewColumn> ListDefinition([FromBody]ListModelDefinitionRequest request)
        {
            FilterBase filter = BusinessBaseProvider.Instance.GetFilter(HttpContext, request.objectName, request.filterName);

            return new DataView(filter).Columns;
        }


        [HttpPost]
        public List<ListItemRest> SimpleList([FromBody]SimpleListRequest request)
        {
            ListTable table = BusinessBaseProvider.ListProvider.GetList(request.objectName,
                request.listName, request.parameter);

            return table.ToClient;
        }

        [HttpPost]
        public JsonResult Post([FromBody]ModelFromClient fromClient)
        {
            try
            {
                if (fromClient.action == "init")
                {
                    ModelToClient toClient = new ModelToClient();

                    toClient.formToken = Guid.NewGuid().ToString();
                    toClient.sequence = 1;
                    toClient.action = "init";

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

                    return new JsonResult(BusinessBaseProvider.RetreiveObject(HttpContext, fromClient.objectName,
                        fromClient.root.key).PerformActionAndCreateResponse(HttpContext, fromClient));
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
