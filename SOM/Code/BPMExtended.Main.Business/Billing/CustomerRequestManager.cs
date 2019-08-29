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

namespace BPMExtended.Main.Business
{
    public class CustomerRequestManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #region Fields

        //ICustomerRequestDataManager s_dataManager = BPMExtendedDataManagerFactory.GetDataManager<ICustomerRequestDataManager>();

        #endregion

        #region Public Methods

        public CreateCustomerRequestOutput ProcessCustomerCreation(BPMCustomerType customerType, Guid accountOrContactId)
        {
            string name = GetContactInfo(accountOrContactId);

            NewCustomerCreationSomRequestSetting newCustomerCreationSomRequestSetting = new NewCustomerCreationSomRequestSetting
            {
                CustomerId = accountOrContactId
            };
            string title = string.Format("New Customer Creation Process Input: '{0}'", newCustomerCreationSomRequestSetting.CustomerId);

            this.UpdateContactCustomerId(accountOrContactId, RatePlanMockDataGenerator.GetRandomCustomerId());

            return Helper.CreateSOMRequest(customerType, accountOrContactId, title, newCustomerCreationSomRequestSetting);
        }

        public List<CustomerRequestHeaderDetail> GetRecentRequestHeaders(int nbOfRecords, BPMCustomerType customerType, Guid accountOrContactId, long? lessThanSequenceNb)
        {
            List<SOMRequestHeader> somRequests = null;
            using(SOMClient client = new SOMClient())
            {
                string entityId = BuildSOMEntityIdFromCustomer(customerType, accountOrContactId);
                somRequests = client.Get<List<SOMRequestHeader>>(String.Format("api/SOM_Main/SOMRequest/GetRecentSOMRequestHeaders?entityId={0}&nbOfRecords={1}&lessThanSequenceNb={2}", entityId, nbOfRecords, lessThanSequenceNb));
            }
            List<CustomerRequestHeaderDetail> customerRequests = new List<CustomerRequestHeaderDetail>();
            if(somRequests != null)
            {
                foreach(var req in somRequests)
                {
                    var customerReq = new CustomerRequestHeaderDetail
                    {
                        CustomerRequestId = req.SOMRequestId,
                        CustomerType = customerType,
                        AccountOrContactId = accountOrContactId,
                        SequenceNumber = req.SequenceNumber,
                        Title = req.Title,
                        RequestTypeId = req.RequestTypeId,
                        Status = MapRequestStatus(req.Status),
                        CreatedTime = req.CreatedTime,
                        LastModifiedTime = req.LastModifiedTime
                    };
                    customerRequests.Add(customerReq);
                }
            }
            return customerRequests;
        }

        public List<CustomerRequestLogDetail> GetRequestLogs(Guid requestId, int nbOfRecords, long? lessThanId)
        {
            List<SOMRequestLog> somLogs = null;
            using(SOMClient client = new SOMClient())
            {
                somLogs = client.Get<List<SOMRequestLog>>(string.Format("api/SOM_Main/SOMRequest/GetSOMRequestLogs?somRequestId={0}&nbOfRecords={1}&lessThanId={2}", requestId, nbOfRecords, lessThanId));
            }
            List<CustomerRequestLogDetail> logs = new List<CustomerRequestLogDetail>();
            foreach(var somLog in somLogs)
            {
                var log = new CustomerRequestLogDetail
                {
                    RequestLogId = somLog.SOMRequestLogId,
                    Severity = somLog.Severity,
                    SeverityDescription = somLog.Severity.ToString(),
                    Message = somLog.Message,
                    ExceptionDetail = somLog.ExceptionDetail,
                    EventTime = somLog.EventTime
                };
                logs.Add(log);
            }
            return logs;
        }

        public List<BPTrackingMessageDetail> GetBPInstanceTrackingMessagesDetails(long processId)
        {
            var BPTrackingMessages = new List<BPTrackingMessageDetail>();

            using (SOMClient client = new SOMClient())
            {
                List<BPTrackingMessage> items = client.Get<List<BPTrackingMessage>>(String.Format("api/SOM.ST/Billing/GetBPInstanceTrackingMessages?processInstanceId={0}", processId));
                foreach (var item in items)
                {
                    var bpItem = BPTrackingMessageToDetailMapper(item);
                    BPTrackingMessages.Add(bpItem);
                }
            }
            return BPTrackingMessages;
        }

        public BPTrackingMessageDetail BPTrackingMessageToDetailMapper(BPTrackingMessage bpTrackingMessage)
        {
            return new BPTrackingMessageDetail()
            {
                Id= bpTrackingMessage.Id,
                ProcessInstanceId = bpTrackingMessage.ProcessInstanceId,
                ParentProcessId = bpTrackingMessage.ParentProcessId,
                //Severity = bpTrackingMessage.Severity,
                TrackingMessage = bpTrackingMessage.TrackingMessage,
                ExceptionDetail = bpTrackingMessage.ExceptionDetail,
                EventTime = bpTrackingMessage.EventTime,
            };

        }
        public CreateCustomerRequestOutput CreateLineSubscriptionRequest(BPMCustomerType customerType, Guid accountOrContactId, LineSubscriptionRequest lineSubscriptionRequest)
        {
            string title = string.Format("Line Subscription '{0}'", lineSubscriptionRequest.PhoneNumber);

            return CreateSOMRequest(customerType, accountOrContactId, title, lineSubscriptionRequest);
            //Guid requestId = Guid.NewGuid();
            //CreateLineSubscriptionInput somRequestInput = new CreateLineSubscriptionInput
            //{
            //    SOMRequestId = requestId,
            //    EntityId = BuildSOMEntityIdFromCustomer(customerObjectType, accountOrContactId),
            //    RequestTitle = title,
            //    RequestDetails = lineSubscriptionRequest
            //};

            //CreateSOMRequestOutput output = null;

            //using (var client = new SOMClient())
            //{
            //    //s_dataManager.Insert(requestId, requestSettings.ConfigId, customerObjectType, accountOrContactId, requestTitle, CustomerRequestStatus.New);//insert request in BPM after making sure connection to SOM succeeds
            //    //try
            //    //{
            //    output = client.Post<CreateLineSubscriptionInput, CreateSOMRequestOutput>("api/SOM_Main/SOMRequest/CreateLineSubscriptionRequest", somRequestInput);
            //    //}
            //    //catch
            //    //{
            //    //    s_dataManager.UpdateRequestStatus(requestId, CustomerRequestStatus.Aborted);
            //    //    throw;
            //    //}
            //}
            //return new CreateCustomerRequestOutput { };
        }

        public CreateCustomerRequestOutput CreateMoveLineRequest(BPMCustomerType customerType, Guid accountOrContactId, MoveLineRequest moveLineRequest)
        {
            string title = string.Format("Move Line '{0}'", moveLineRequest.PhoneNumber);
            return CreateSOMRequest(customerType, accountOrContactId, title, moveLineRequest);
        }

        public CreateCustomerRequestOutput CreateLineSubscriptionTerminationRequest(BPMCustomerType customerType, Guid accountOrContactId, LineSubscriptionTerminationProcessRequest lineSubscriptionTerminationRequest)
        {
            string title = string.Format("Line Subscription Termination '{0}'", lineSubscriptionTerminationRequest.PhoneNumber);
            return CreateSOMRequest(customerType, accountOrContactId, title, lineSubscriptionTerminationRequest);
        }

        //public void UpdateRequestStatus(Guid requestId, CustomerRequestStatus status)
        //{
        //    s_dataManager.UpdateRequestStatus(requestId, status);
        //}



        public string CreateWorkOrder(string requestId, string WorkOrderType)
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
                workOrderId = initiateWorkOrder(requestId,requestsTypeId,WorkOrderType,recordName);

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
                    recordEntity.SetColumnValue(TechnicalStepFieldName, WorkOrderType);
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
                    recordEntity.SetColumnValue("StWorkOrderStageId", WorkOrderType);
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
            string recordName= "";

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
        private string initiateWorkOrder(string requestId, string requestTypeId,string workOrderType, string recordName)
        {
            EntitySchema schema = BPM_UserConnection.EntitySchemaManager.GetInstanceByName("StWorkOrder");
            Entity workorder = schema.CreateEntity(BPM_UserConnection);

            Guid workOrderId = Guid.NewGuid();
            string workOrderName = new CommonManager().GetWorkOrderTypeName(workOrderType);

            workorder.SetColumnValue("Id", workOrderId);
            workorder.SetColumnValue("StName", workOrderName+"Step for: " + recordName);
            workorder.SetColumnValue("StDescription", workOrderName+"Step for: " + recordName);
            workorder.SetColumnValue("StRequestID", requestId);
            workorder.SetColumnValue("StStatusId", "7470FB2F-4701-488D-99B2-F7A71400CB9E");
            workorder.SetColumnValue("StRequestTypeId", requestTypeId);
            workorder.SetColumnValue("StTypeId", workOrderType); //"2A6D299E-A312-479A-962F-C711221DBDC2");
            workorder.Save();

            return workOrderId.ToString();
        }
        #endregion

        public string SetRequestCompleted(string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            string SchemaName = "";
            string CompletedStepId = "";
            string CompletedStep = "";
            string TechnicalStep = "";

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
                TechnicalStepFieldNameAttribute technicalStepField = Utilities.GetEnumAttribute<OperationType, TechnicalStepFieldNameAttribute>((OperationType)reqcode);
                SchemaName = Utilities.GetEnumAttribute<OperationType, EntitySchemaNameAttribute>((OperationType)reqcode).schemaName;
                CompletedStepId = Utilities.GetEnumAttribute<OperationType, CompletedStepIdAttribute>((OperationType)reqcode).CompletedStepId;
                CompletedStep = Utilities.GetEnumAttribute<OperationType, CompletedStepAttribute>((OperationType)reqcode).CompletedStep;
                TechnicalStep = technicalStepField==null?null: Utilities.GetEnumAttribute<OperationType, TechnicalStepFieldNameAttribute>((OperationType)reqcode).fieldName;

                UpdateRequestStatus(requestId, SchemaName,CompletedStepId,CompletedStep, TechnicalStep);
            }
            return "";
        }

        public string CancelRequestHeader(string requestId)
        {
            var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StRequestHeader");
            var recordEntity = recordSchema.CreateEntity(UserConnection);

            var eSQ = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            eSQ.RowCount = 1;
            eSQ.AddAllSchemaColumns();
            eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId));
            var collection = eSQ.GetEntityCollection(UserConnection);

            if (collection.Count > 0)
            {
                if (collection.Count > 0)
                {
                    recordEntity = collection[0];
                    recordEntity.SetColumnValue("StStatusId", "87486F6C-FD2B-498D-8E4A-90935299058E");
                }
                recordEntity.Save();
            }
            return "";
        }

        public List<SOMProcessInstanceLog> GetFulfillmentLogs(Guid requestId)
        {
            var processInstancesLogs = new List<SOMProcessInstanceLog>();

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StSOMProcessInstancesLogs");
            esq.AddColumn("Id");
            esq.AddColumn("StRequestId");
            esq.AddColumn("StProcessInstanceId");
            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
            esq.Filters.Add(esqFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            for (int i = 0; i < entities.Count; i++)
            {
                var processInstanceLog = new SOMProcessInstanceLog()
                {
                    ProcessInstanceId = (int)entities[i].GetColumnValue("StProcessInstanceId"),
                    RequestId = requestId
                };
                processInstancesLogs.Add(processInstanceLog);
            }

            return processInstancesLogs;
        }


        private void UpdateRequestStatus(string requestId, string SchemaName, string CompletedStepId,string CompletedStep, string TechnicalStep)
        {
            //TODO : Update gshdsl object
            var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName(SchemaName);
            var recordEntity = recordSchema.CreateEntity(UserConnection);

            var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, SchemaName);
            eSQ.RowCount = 1;
            eSQ.AddAllSchemaColumns();
            eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
            var collection = eSQ.GetEntityCollection(UserConnection);
            if (collection.Count > 0)
            {
                recordEntity = collection[0];
                recordEntity.SetColumnValue(CompletedStep, CompletedStepId);
                if(TechnicalStep != null) recordEntity.SetColumnValue(TechnicalStep, null);
            }
            recordEntity.Save();
        }

        #region Private Methods

        private CreateCustomerRequestOutput CreateSOMRequest(BPMCustomerType customerType, Guid accountOrContactId, string requestTitle, SOMRequestExtendedSettings requestSettings)
        {
            Guid requestId = Guid.NewGuid();
            CreateSOMRequestInput somRequestInput = new CreateSOMRequestInput
             {
                 SOMRequestId = requestId,
                 EntityId = BuildSOMEntityIdFromCustomer(customerType, accountOrContactId),
                 RequestTitle = requestTitle,
                 Settings = new SOMRequestSettings { ExtendedSettings = requestSettings }
             };

            CreateSOMRequestOutput output = null;

            using (var client = new SOMClient())
            {
                //s_dataManager.Insert(requestId, requestSettings.ConfigId, customerObjectType, accountOrContactId, requestTitle, CustomerRequestStatus.New);//insert request in BPM after making sure connection to SOM succeeds
                //try
                //{
                    output = client.Post<CreateSOMRequestInput, CreateSOMRequestOutput>("api/SOM_Main/SOMRequest/CreateSOMRequest", somRequestInput);
                //}
                //catch
                //{
                //    s_dataManager.UpdateRequestStatus(requestId, CustomerRequestStatus.Aborted);
                //    throw;
                //}
            }
            return new CreateCustomerRequestOutput
            {
            };
        }

        private static string BuildSOMEntityIdFromCustomer(BPMCustomerType customerType, Guid accountOrContactId)
        {
            return String.Concat(customerType.ToString(), "_", accountOrContactId.ToString());
        }

        //private CustomerRequestDetail CustomerRequestDetailMapper(CustomerRequest request)
        //{
        //    var requestDetail = new CustomerRequestDetail
        //    {
        //        CustomerRequestId = request.CustomerRequestId,
        //        CustomerObjectType = request.CustomerObjectType,
        //        AccountOrContactId = request.AccountOrContactId,
        //        RequestTypeId = request.RequestTypeId,
        //        Title = request.Title,
        //        Status = request.Status,
        //        SequenceNumber = request.SequenceNumber,
        //        CreatedTime = request.CreatedTime,
        //        LastModifiedTime = request.LastModifiedTime
        //    };
        //    return requestDetail;
        //}

        private CustomerRequestStatus MapRequestStatus(SOMRequestStatus somRequestStatus)
        {
            switch (somRequestStatus)
            {
                case SOMRequestStatus.New: return CustomerRequestStatus.New;
                case SOMRequestStatus.Running: return CustomerRequestStatus.Running;
                case SOMRequestStatus.Waiting: return CustomerRequestStatus.Waiting;
                case SOMRequestStatus.Completed: return CustomerRequestStatus.Completed;
                case SOMRequestStatus.Aborted: return CustomerRequestStatus.Aborted;
                default: throw new NotSupportedException(String.Format("somRequestStatus '{0}'", somRequestStatus.ToString()));
            }
        }

        private string GetContactInfo(Guid contactId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var esqResult = new EntitySchemaQuery(connection.EntitySchemaManager, "Contact");
            esqResult.AddColumn("Name");

            // Execution of query to database and getting object with set identifier.
            var entity = esqResult.GetEntity(connection, contactId);
            object retVal = entity.GetColumnValue("Name");

            return retVal != null ? retVal.ToString() : null;            
        }

        private void UpdateContactCustomerId(Guid contactId, string customerId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            //var update = new Update(connection, "Contact").Set("StCustomerId", Func.IsNull(Column.SourceColumn("StCustomerId"), Column.Parameter(contactId)));
            var update = new Update(connection, "Contact").Set("StCustomerId", Column.Parameter(customerId))
                .Where("Id").IsEqual(Column.Parameter(contactId));
            update.Execute();
        }

        #endregion
    }
}
