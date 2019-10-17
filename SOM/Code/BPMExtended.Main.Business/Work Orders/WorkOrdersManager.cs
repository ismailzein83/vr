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
        //public string CreateWorkOrder(string requestId, string WorkOrderType)
        //{
        //    EntitySchemaQuery esq;
        //    IEntitySchemaQueryFilterItem esqFirstFilter;
        //    UserConnection UserConnection;
        //    EntitySchema recordSchema;
        //    Entity recordEntity;
        //    string EntityName = "";
        //    string workOrderId = "";
        //    string StepFieldName = "";
        //    string TechnicalStepFieldName = "";
        //    string TechnicalStepId = "";
        //    string stageId = "";

        //    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
        //    esq.AddColumn("StRequestId");
        //    var requestTypeId = esq.AddColumn("StRequestType");

        //    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
        //    esq.Filters.Add(esqFirstFilter);

        //    var entities = esq.GetEntityCollection(BPM_UserConnection);

        //    if (entities.Count > 0)
        //    {
        //        int reqcode = 0;
        //        int.TryParse(entities[0].GetTypedColumnValue<int>(requestTypeId.Name).ToString(), out reqcode);
        //        EntityName = Utilities.GetEnumAttribute<OperationType, EntitySchemaNameAttribute>((OperationType)reqcode).schemaName;
        //        TechnicalStepId = Utilities.GetEnumAttribute<OperationType, TechnicalStepIdAttribute>((OperationType)reqcode).technicalStepId;
        //        StepFieldName = Utilities.GetEnumAttribute<OperationType, CompletedStepAttribute>((OperationType)reqcode).CompletedStep;
        //        TechnicalStepFieldName = Utilities.GetEnumAttribute<OperationType, TechnicalStepFieldNameAttribute>((OperationType)reqcode).fieldName;
        //        string requestsTypeId = GetRequestType(EntityName);
        //        string recordName = GetEntityName(EntityName, requestId);
        //        workOrderId = initiateWorkOrder(requestId, requestsTypeId, WorkOrderType, recordName);

        //        //update request
        //        UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
        //        recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName(EntityName);
        //        recordEntity = recordSchema.CreateEntity(UserConnection);

        //        var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, EntityName);
        //        eSQ.RowCount = 1;
        //        eSQ.AddAllSchemaColumns();
        //        eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
        //        var collection = eSQ.GetEntityCollection(UserConnection);
        //        if (collection.Count > 0)
        //        {
        //            recordEntity = collection[0];
        //            recordEntity.SetColumnValue(TechnicalStepFieldName, WorkOrderType);
        //            recordEntity.SetColumnValue(StepFieldName, TechnicalStepId);
        //            recordEntity.SetColumnValue("StWorkOrderID", workOrderId);
        //            recordEntity.SetColumnValue("StIsWorkOrderCompleted", false);
        //        }
        //        recordEntity.Save();

        //        //get stage id
        //        esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequeststage");
        //        var IdCol = esq.AddColumn("Id");

        //        esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStageId", TechnicalStepId);
        //        esq.Filters.Add(esqFirstFilter);

        //        var items = esq.GetEntityCollection(BPM_UserConnection);
        //        if (items.Count > 0)
        //        {
        //            stageId = items[0].GetTypedColumnValue<Guid>(IdCol.Name).ToString();

        //        }

        //        //update request header table
        //        UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
        //        recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StRequestHeader");
        //        recordEntity = recordSchema.CreateEntity(UserConnection);

        //        esq = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "StRequestHeader");
        //        esq.RowCount = 1;
        //        esq.AddAllSchemaColumns();
        //        esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId));
        //        collection = esq.GetEntityCollection(UserConnection);
        //        if (collection.Count > 0)
        //        {
        //            recordEntity = collection[0];
        //            recordEntity.SetColumnValue("StWorkOrderStageId", WorkOrderType);
        //            recordEntity.SetColumnValue("StStageId", stageId);
        //        }
        //        recordEntity.Save();

        //    }



        //    return workOrderId;
        //}
    }
}
