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

namespace BPMExtended.Main.Business
{
    public class CustomerRequestManager
    {
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

            this.UpdateContactCustomerId(accountOrContactId, MockDataGenerator.GetRandomCustomerId());

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

        #endregion

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
