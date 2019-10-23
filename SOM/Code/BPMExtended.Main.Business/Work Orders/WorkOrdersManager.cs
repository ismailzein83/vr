using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.BP.Arguments;
using SOM.Main.Entities;
using BPMExtended.Main.Data;
using BPMExtended.Main.Common;
using Terrasoft.Core.Entities;
using Terrasoft.Core;
using System.Web;
using Terrasoft.Core.DB;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core.Process;

namespace BPMExtended.Main.Business
{
    public class WorkOrdersManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public string CreateWorkOrder(WorkOrder w)
        {
            return "success";
        }
        public string CreateWorkOrders(string type, string requestId, string pathId, string phoneNumber, string switchName, string deviceName, bool supportsCommand, string commands)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            UserConnection UserConnection;
            EntitySchema recordSchema;
            Entity recordEntity;
            string EntityName = "";
            string workOrderId = "";
            string StepFieldName = "";
            string TechnicalStepFieldName = "";
            string TechnicalStepId = "";
            string stageId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StRequestId");
            var requestTypeId = esq.AddColumn("StRequestType");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                int reqcode = 0;
                int.TryParse(entities[0].GetTypedColumnValue<int>(requestTypeId.Name).ToString(), out reqcode);
                EntityName = Utilities.GetEnumAttribute<OperationType, EntitySchemaNameAttribute>((OperationType)reqcode).schemaName;
                TechnicalStepId = Utilities.GetEnumAttribute<OperationType, TechnicalStepIdAttribute>((OperationType)reqcode).technicalStepId;
                StepFieldName = Utilities.GetEnumAttribute<OperationType, CompletedStepAttribute>((OperationType)reqcode).CompletedStep;
                TechnicalStepFieldName = Utilities.GetEnumAttribute<OperationType, TechnicalStepFieldNameAttribute>((OperationType)reqcode).fieldName;
                string requestsTypeId = GetRequestType(EntityName);
                string recordName = GetEntityName(EntityName, requestId);
                workOrderId = initiateWorkOrder(requestId, requestsTypeId, type, recordName);

                //update request
                UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName(EntityName);
                recordEntity = recordSchema.CreateEntity(UserConnection);

                var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, EntityName);
                eSQ.RowCount = 1;
                eSQ.AddAllSchemaColumns();
                eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
                var collection = eSQ.GetEntityCollection(UserConnection);
                if (collection.Count > 0)
                {
                    recordEntity = collection[0];
                    recordEntity.SetColumnValue(TechnicalStepFieldName, type);
                    recordEntity.SetColumnValue(StepFieldName, TechnicalStepId);
                    recordEntity.SetColumnValue("StWorkOrderID", workOrderId);
                    recordEntity.SetColumnValue("StIsWorkOrderCompleted", false);
                }
                recordEntity.Save();

                //get stage id
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequeststage");
                var IdCol = esq.AddColumn("Id");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStageId", TechnicalStepId);
                esq.Filters.Add(esqFirstFilter);

                var items = esq.GetEntityCollection(BPM_UserConnection);
                if (items.Count > 0)
                {
                    stageId = items[0].GetTypedColumnValue<Guid>(IdCol.Name).ToString();

                }

                //update request header table
                UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StRequestHeader");
                recordEntity = recordSchema.CreateEntity(UserConnection);

                esq = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "StRequestHeader");
                esq.RowCount = 1;
                esq.AddAllSchemaColumns();
                esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId));
                collection = esq.GetEntityCollection(UserConnection);
                if (collection.Count > 0)
                {
                    recordEntity = collection[0];
                    recordEntity.SetColumnValue("StWorkOrderStageId", type);
                    recordEntity.SetColumnValue("StStageId", stageId);
                }
                recordEntity.Save();

            }



            return workOrderId;
        }
        private string GetEntityName(string entityName, string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            string recordName = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            esq.AddColumn("StName");
            var Id = esq.AddColumn("Id");
            var name = esq.AddColumn("StName");
            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                recordName = entities[0].GetTypedColumnValue<string>(name.Name);
            }
            return recordName;
        }
        private string GetRequestType(string EntityName)
        {
            string requestTypeId = "";
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            //string EntityName = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestsTypes");
            esq.AddColumn("StEntityName");
            var Id = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StEntityName", EntityName);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                requestTypeId = entities[0].GetTypedColumnValue<Guid>(Id.Name).ToString();
            }
            return requestTypeId;
        }
        private string initiateWorkOrder(string requestId, string requestTypeId, string workOrderType, string recordName)
        {
            EntitySchema schema = BPM_UserConnection.EntitySchemaManager.GetInstanceByName("StWorkOrder");
            Entity workorder = schema.CreateEntity(BPM_UserConnection);

            Guid workOrderId = Guid.NewGuid();
            string workOrderName = new CommonManager().GetWorkOrderTypeName(workOrderType);

            workorder.SetColumnValue("Id", workOrderId);
            workorder.SetColumnValue("StName", workOrderName + "Step for: " + recordName);
            workorder.SetColumnValue("StDescription", workOrderName + "Step for: " + recordName);
            workorder.SetColumnValue("StRequestID", requestId);
            workorder.SetColumnValue("StStatusId", "7470FB2F-4701-488D-99B2-F7A71400CB9E");
            workorder.SetColumnValue("StRequestTypeId", requestTypeId);
            workorder.SetColumnValue("StTypeId", workOrderType); //"2A6D299E-A312-479A-962F-C711221DBDC2");
            workorder.Save();

            return workOrderId.ToString();
        }

    }
}
