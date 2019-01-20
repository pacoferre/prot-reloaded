using PROTR.Core.Lists;
using PROTR.Core.REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core
{
    public partial class BusinessBase
    {
        public virtual async Task<ModelToClient> PerformActionAndCreateResponse(HttpContext context, ModelFromClient fromClient)
        {
            ModelToClient model = new ModelToClient
            {
                wasNew = IsNew,
                wasDeleting = IsDeleting,
                wasModified = IsModified
            };

            if (fromClient.action == "load")
            {
                await ReadFromDB();
                model.refreshAll = true;
            }
            else if (fromClient.action == "new")
            {
                if (!IsNew)
                {
                    // Almost innecesary, done by BusinessBaseProvider.RetreiveObject in CRUDController.
                    await SetNew();
                }
            }
            else if (fromClient.action == "delete")
            {
                IsDeleting = true;
            }
            else if (fromClient.action == "changed" || fromClient.action == "ok")
            {
                foreach (KeyValuePair<string, string> item in fromClient.root.data)
                {
                    PropertyDefinition prop = Decorator.Properties[item.Key];

                    await prop.SetValue(this, item.Value);
                }

                await ProcessCollectionsFromClient(context, fromClient, model);

                if (fromClient.action == "ok")
                {
                    model.refreshAll = true;
                    try
                    {
                        string messageAction = IsDeleting ? "deleted" : (IsNew ? "created" : "saved");

                        await contextProvider.DbContext.BeginTransactionAsync();
                        await StoreToDB();

                        model.normalMessage = Description + " " + messageAction + " successfully.";

                        contextProvider.DbContext.CommitTransaction();
                    }
                    catch (Exception exp)
                    {
                        contextProvider.DbContext.RollbackTransaction();

                        model.ok = false;
                        model.errorMessage = LastErrorMessage == "" ? exp.Message : LastErrorMessage;
                    }
                }
            }
            else if (fromClient.action == "clear")
            {
                if (IsNew)
                {
                    await SetNew();
                }
                else
                {
                    if (IsDeleting)
                    {
                        IsDeleting = false;
                    }

                    await ReadFromDB();
                }

                model.refreshAll = true;
            }

            // Send object data.
            model.data = new Dictionary<string, string>(Decorator.ListProperties.Count);
            if (fromClient.dataNames != null)
            {
                for (int index = 0; index < fromClient.dataNames.Count; ++index)
                {
                    PropertyDefinition prop = Decorator.Properties[fromClient.dataNames[index]];

                    model.data.Add(prop.FieldName, await prop.GetValue(this));
                }
            }
            await ProcessCollectionsToClient(context, fromClient, model);

            await businessProvider.StoreObject(this, fromClient.objectName);

            model.keyObject = Key;
            model.isNew = IsNew;
            model.isModified = IsModified;
            model.isDeleting = IsDeleting;

            if (ClientRefreshPending && !model.refreshAll)
            {
                model.refreshAll = true;
            }

            model.title = Title;

            model.action = fromClient.action;

            SetExtraToClientResponse(model);

            model.Sanitize();

            return model;
        }

        protected virtual void SetExtraToClientResponse(ModelToClient model)
        {

        }

        private async Task ProcessCollectionsFromClient(HttpContext context, ModelFromClient fromClient, ModelToClient model)
        {
            if (fromClient.root.children != null && fromClient.root.children.Count > 0)
            {
                model.collections = new Dictionary<string, List<ModelToClient>>(fromClient.root.children.Count);

                foreach (ModelFromClientCollection clientCol in fromClient.root.children)
                {
                    BusinessCollectionBase col = await Collection(clientCol.path);
                    List<ModelToClient> elements = new List<ModelToClient>(col.Count);
                    ModelFromClientData clientElement;

                    foreach (BusinessBase obj in col)
                    {
                        clientElement = null;

                        if (clientCol.elements != null && !model.refreshAll)
                        {
                            clientElement = clientCol.elements.Find(element => obj.Key == element.key);
                        }

                        elements.Add(await obj.ProcessRequestInternalElement(context, clientCol, clientElement, fromClient.action));
                    }

                    model.collections.Add(clientCol.path, elements);
                }
            }
        }

        private async Task ProcessCollectionsToClient(HttpContext context, ModelFromClient fromClient, ModelToClient model)
        {
            if (fromClient.root.children != null && fromClient.root.children.Count > 0)
            {
                if (model.refreshAll)
                {
                    model.collections = null;
                }
                if (model.collections == null)
                {
                    await ProcessCollectionsFromClient(context, fromClient, model);
                }
                foreach (ModelFromClientCollection clientCol in fromClient.root.children)
                {
                    BusinessCollectionBase col = await Collection(clientCol.path);
                    List<ModelToClient> elements = model.collections[clientCol.path];
                    ModelToClient currentModel = null;

                    foreach (BusinessBase obj in col)
                    {
                        currentModel = elements.Find(element => element.keyObject == obj.Key);

                        await obj.ProcessResponseInternalElement(context, clientCol, currentModel);
                    }
                }
            }
        }

        public async Task<ModelToClient> ProcessRequestInternalElement(HttpContext context, ModelFromClientCollection fromClient,
            ModelFromClientData element, string fromClientAction)
        {
            ModelToClient model = new ModelToClient
            {
                wasNew = IsNew,
                wasDeleting = IsDeleting,
                wasModified = IsModified,
                keyObject = Key
            };

            // Too much copy/paste
            if (element != null)
            {
                if (fromClientAction == "changed" || fromClientAction == "ok")
                {
                    if (element.data != null)
                    {
                        foreach (KeyValuePair<string, string> item in element.data)
                        {
                            PropertyDefinition prop = Decorator.Properties[item.Key];

                            await prop.SetValue(this, item.Value);
                        }
                    }
                }
            }

            return model;
        }

        private async Task ProcessResponseInternalElement(HttpContext context, 
            ModelFromClientCollection fromClient,
            ModelToClient model)
        {
            // Send object data.
            model.data = new Dictionary<string, string>(Decorator.ListProperties.Count);
            if (fromClient.dataNames != null)
            {
                for (int index = 0; index < fromClient.dataNames.Count; ++index)
                {
                    PropertyDefinition prop = Decorator.Properties[fromClient.dataNames[index]];

                    model.data.Add(prop.FieldName, await prop.GetValue(this));
                }
            }

            model.isNew = IsNew;
            model.isModified = IsModified;
            model.isDeleting = IsDeleting;

            if (clientRefreshPending && !model.refreshAll)
            {
                model.refreshAll = true;
            }

            model.title = Title;

            // First level now...
            //if (relatedCollections.Count > 0)
            //{
            //    model.collections = new Dictionary<string, List<ModelToClient>>(relatedCollections.Count);

            //    foreach (BusinessCollectionBase col in relatedCollections.Values)
            //    {
            //        List<ModelToClient> elements = new List<ModelToClient>(col.Count);

            //        foreach (BusinessBase obj in col)
            //        {
            //            elements.Add(obj.CreateResponse(null, null));
            //        }
            //    }
            //}
        }

        private bool clientRefreshPending = false;
        public virtual bool ClientRefreshPending
        {
            get
            {
                return clientRefreshPending;
            }
            set
            {
                clientRefreshPending = value;
            }
        }
    }
}
