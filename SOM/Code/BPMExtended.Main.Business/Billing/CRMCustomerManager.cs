﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using SOM.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CRMCustomerManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection 
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public

        public bool IsCustomerCategoryNormal(string customerCategoryId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            bool isNormal = false;

            //Call Categories catalog and check the 'IsNormal' field if true => no need for attachments (optional), if false => attachment is required 
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StDefault");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryID", customerCategoryId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isNormal = (bool)entities[0].GetColumnValue("StDefault");

            }


            return isNormal;
        }

        public OutputResult NeedsAttachment(string contactId, string accountId, string customerCategoryId)
        {
            bool isNormal;
            string msg = "";
            ResultStatus status = ResultStatus.Success;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            isNormal = IsCustomerCategoryNormal(customerCategoryId);

            if (isNormal)
            {
                msg = "No need for attachments";
                status = ResultStatus.Success;

            }
            else
            {

                if (contactId != null)
                {
                    //need attachment
                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "ContactFile");
                    esq.AddColumn("Id");

                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Contact.Id", contactId);
                    esq.Filters.Add(esqFirstFilter);
                    entities = esq.GetEntityCollection(BPM_UserConnection);

                    if (entities.Count > 0)
                    {
                        msg = "Attachment exist";
                        status = ResultStatus.Success;
                    }
                    else
                    {
                        msg = "Please Add Attachments";
                        status = ResultStatus.Error;
                    }


                }
                else
                {
                    //account
                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "AccountFile");
                    esq.AddColumn("Id");

                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Account.Id", accountId);
                    esq.Filters.Add(esqFirstFilter);
                    entities = esq.GetEntityCollection(BPM_UserConnection);

                    if (entities.Count > 0)
                    {
                        msg = "Attachment exist";
                        status = ResultStatus.Success;
                    }
                    else
                    {
                        msg = "Please Add Attachments";
                        status = ResultStatus.Error;
                    }

                }

            }

            return new OutputResult()
            {
                messages = new List<string>() { msg },
                status = status
            };
        }

        public Customer GetCustomerInfo(string customerId)
        {
            var item = new Customer();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<Customer>(String.Format("api/SOM.ST/Billing/GetCustomer?CustomerId={0}", customerId));
            }
            return item;
        }

        public CustomerAddress GetCustomerAddress(string customerId)
        {
            var item = new CustomerAddress();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<CustomerAddress>(String.Format("api/SOM.ST/Billing/GetCustomerAddress?CustomerId={0}", customerId));
            }
            return item;
        }


        public bool WaitingListPaymentValidation(string customerId, string contractId)
        {
            return true;
        }
        
        public bool AddDepositForWaitingList(string customerId, string contractId)
        {
            return true;
        }

        public List<CRMCustomerInfoLabelAutoGenerated> GetProfileCRMCustomerInfoAutoGenerated(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<CRMCustomerInfoLabelAutoGenerated> customerInfo = new List<CRMCustomerInfoLabelAutoGenerated>();

            //Get infos from contact table in CRM database 

            if (contactId != null)
            {
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("Name");
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Name", Description = name.ToString() });
                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Document Id", Description = documentId.ToString() });
                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Category", Description = customerCategoryName.ToString() });

                }

            }
            else
            {
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Account");
                esq.AddColumn("Name");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", accountId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var name = entities[0].GetColumnValue("Name");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Name", Description = name.ToString() });
                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Category", Description = customerCategoryName.ToString() });

                }

            }


            return customerInfo;

        }

        public CSO GetCSOByContactIdOrAccountId(string contactId, string accountId)
        {

            var customerInfo = GetCRMCustomerInfo(contactId, accountId);

            var csoId = customerInfo!=null?customerInfo.csoId:null;
            string csoName = null;

            if (csoId != null)
            {
                csoName = GetCSONameById(csoId);
            }

            var cso = new CSO()
            {
                Id = csoId,
                Name = csoName
            };

            return cso;
        }

        public string GetCSONameById(string csoId)
        {
            string csoName = null;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCSO");
            esq.AddColumn("StName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", csoId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                csoName = (string)entities[0].GetColumnValue("StName");
            }
            return csoName;
        }

        public void WaitingListPoolRemoveProcess(string recordId)
        {
            string requestId = "";
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StWaitingListPool");
            esq.AddColumn("StRequestId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", recordId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                requestId = (string)entities[0].GetColumnValue("StRequestId");
                new CommonManager().AbortRequest(requestId);
            }
        }

        public void CancelRecordInWaitingListPoolIfExist(string requestId)
        {
            var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("StWaitingListPool");
            var recordEntity = recordSchema.CreateEntity(UserConnection);

            var eSQ = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StWaitingListPool");
            eSQ.RowCount = 1;
            eSQ.AddAllSchemaColumns();
            eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId));
            var collections = eSQ.GetEntityCollection(UserConnection);

            if (collections != null)
            {
                if (collections.Count > 0)
                {
                    recordEntity = collections[0];
                    recordEntity.SetColumnValue("StWaitingListPoolstatusId", Constant.REMOVED_STATUS);
                    recordEntity.Save();
                }
            }
        }

        public CRMCustomerInfo GetCRMCustomerInfo(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            CRMCustomerInfo customerInfo = null;
            //Get infos from contact table in CRM database 

            if (contactId != null)
            {
                //PostLineSubscriptionToOM(new Guid("CAA17BBE-5D7B-4127-92B9-021417B0AC50"));
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("Name");
                var phonecol = esq.AddColumn("MobilePhone");
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCSO");
                esq.AddColumn("StCSO.Id");
                var csoBSCSIdcol = esq.AddColumn("StCSO.StCSOBSCSId");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryID");
                esq.AddColumn("StCustomerCategoryName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var csoId = entities[0].GetColumnValue("StCSOId");
                    var csoBSCSId = entities[0].GetTypedColumnValue<string>(csoBSCSIdcol.Name);
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");
                    var phone = entities[0].GetTypedColumnValue<string>(phonecol.Name);

                    customerInfo = new CRMCustomerInfo()
                    {
                        CustomerId = customerId.ToString(),
                        CustomerName = name.ToString(),
                        DocumentID = documentId.ToString(),
                        CustomerCategoryID = customerCategoryId.ToString(),
                        CustomerCategoryName = customerCategoryName.ToString(),
                        csoId = csoId.ToString(),
                        csoBSCSId = csoBSCSId.ToString(),
                        PhoneNumber = phone


                    };
                }

            }
            else
            {
                //account
                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Account");
                    esq.AddColumn("Name");
                    esq.AddColumn("StCSO");
                    esq.AddColumn("StCSO.Id");
                    var csoBSCSIdcol = esq.AddColumn("StCSO.StCSOBSCSId");
                    esq.AddColumn("StCustomerId");
                    esq.AddColumn("StCustomerCategoryID");
                    esq.AddColumn("StCustomerCategoryName");

                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", accountId);
                    esq.Filters.Add(esqFirstFilter);

                    var entities = esq.GetEntityCollection(BPM_UserConnection);
                    if (entities.Count > 0)
                    {
                        var csoId = entities[0].GetColumnValue("StCSOId");
                        var csoBSCSId = entities[0].GetTypedColumnValue<string>(csoBSCSIdcol.Name);
                        var name = entities[0].GetColumnValue("Name");
                        var customerId = entities[0].GetColumnValue("StCustomerId");
                        var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                        var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                        customerInfo = new CRMCustomerInfo()
                        {
                            CustomerId = customerId.ToString(),
                            CustomerName = name.ToString(),
                            CustomerCategoryID = customerCategoryId.ToString(),
                            CustomerCategoryName = customerCategoryName.ToString(),
                            csoId = csoId.ToString(),
                            csoBSCSId = csoBSCSId.ToString()


                        };
                    }
            }


            return customerInfo;

        }
        

        public OutputResult ValidateRequest(string contactId, string accountId)
        {
            bool isSkip = false;
            string msgCode = "";
            ResultStatus status;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            CRMCustomerInfo customerInfo;

            customerInfo = new CRMCustomerManager().GetCRMCustomerInfo(contactId, accountId);

            // check categories catalog
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StSkipBalanceCheck");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryID", customerInfo.CustomerCategoryID);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isSkip = (bool)entities[0].GetColumnValue("StSkipBalanceCheck");
            }

            if (isSkip)
            {
                msgCode = Constant.VALIDATION_BALANCE_VALID;
                status = ResultStatus.Success;
            }
            else
            {
                //check customer balance

                //if (new BillingManager().GetCustomerBalance(info.CustomerId).Balance > 0)
                if (new Random().Next(-10, 10) > 0)
                {
                    msgCode = Constant.VALIDATION_BALANCE_NOT_VALID;
                    status = ResultStatus.Error;
                }
                else
                {
                    msgCode = Constant.VALIDATION_BALANCE_VALID;
                    status = ResultStatus.Success;
                }

            }

            return new OutputResult()
            {
                messages = new List<string>() { msgCode },
                status = status
            };

        }
        public bool ValidatePayment(string sequenceNumber)
        {
            //TODO: check if user has paid
            return true;
        }


        public CSO GetCustomerCSO(string sysUserId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            CSO csoObject = null;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysUserInRole");
            esq.AddColumn("SysUser");
           // var cashierColumn = esq.AddColumn("SysUser.Contact.StCSO.StCashier");
            var csoName = esq.AddColumn("SysUser.Contact.StUserCSO.StName");
            var csoId = esq.AddColumn("SysUser.Contact.StUserCSO.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysUser", sysUserId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                string id = entities[0].GetTypedColumnValue<string>(csoId.Name);
                string name = entities[0].GetTypedColumnValue<string>(csoName.Name);

                csoObject = new CSO()
                    {
                        Id = id,
                        Name = name
                };
            }

            return csoObject;
        }

        public bool IsOperationNeedProvisioning(string opType)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter, esqFirstFilter2;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StOperationSetting");
            esq.AddColumn("Id");
            esq.AddColumn("StDocumentID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCustomerCategoryID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StOperationType", opType);
            esqFirstFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StNeedProvisioning", "1");

            esq.Filters.Add(esqFirstFilter);
            esq.Filters.Add(esqFirstFilter2);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public PaymentMethod CheckCSO(string sysUserId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysAdminUnit");
            esq.AddColumn("Id");
            var cashierColumn = esq.AddColumn("Contact.StUserCSO.StCashier");

            //var csoidcol = esq.AddColumn("StCSO.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", sysUserId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                bool isCashier = entities[0].GetTypedColumnValue<bool>(cashierColumn.Name);

                if (isCashier)
                    return new PaymentMethod()
                    {
                        Id = "E78BC2E8-119B-475B-AFB1-962B11842597",
                        Name = "Cash"
                    };

                else
                    return new PaymentMethod()
                    {
                        Id = "E9F030F4-BF08-4E99-96C2-86C92134FC0F",
                        Name = "Invoice"
                    };

            }

            return null;
        }
        public bool IsCommercial(string sysUserId)
        {
            bool isCommercial = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysAdminUnit");
            esq.AddColumn("Id");
            var commercialColumn = esq.AddColumn("Contact.StUserCSO.StCommercial");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", sysUserId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                isCommercial = entities[0].GetTypedColumnValue<bool>(commercialColumn.Name);
            }

            return isCommercial;
        }


        public string GetSequenceNumberFromRequestHeader(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string sequenceNumber=null;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StSequenceNumber");
            esq.AddColumn("StRequestId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);


            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                sequenceNumber = entities[0].GetTypedColumnValue<string>("StSequenceNumber");

            }

            return sequenceNumber;

        }


        public List<RequestHeaderDetail> GetRequestHeaderData(string contactId, string accountId, string contractId, string requestId, string from, string to)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            IEntitySchemaQueryFilterItem esqSecondFilter;
            IEntitySchemaQueryFilterItem esqFromFilter;
            IEntitySchemaQueryFilterItem esqToFilter;
            EntityCollection entities;
            List<RequestHeaderDetail> requests = new List<RequestHeaderDetail>();

            var fromDate = new DateTime();
            var toDate = new DateTime();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StStep");
            esq.AddColumn("StTechnicalStep");
            esq.AddColumn("StRequestId");
            esq.AddColumn("StStatus");
            esq.AddColumn("StContractID");
            esq.AddColumn("StRequestType");
            var createdoncol = esq.AddColumn("CreatedOn");
            createdoncol.OrderPosition = 0;
            createdoncol.OrderDirection = Terrasoft.Common.OrderDirection.Descending;

            esq.AddColumn("StSequenceNumber");
            var stagecol = esq.AddColumn("StStage.StName");
            var wostagecol = esq.AddColumn("StWorkOrderStage.Name");
            esq.AddColumn("StSequenceNumber");
            var createdbycol = esq.AddColumn("CreatedBy.Name");

            // var c = esq.AddColumn("StContact");
            // var contact = esq.AddColumn("StContact.Id");

            if (contactId != null && contactId!="") { 
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContact", contactId);
            }
            else if (accountId != null && accountId!="")
            {
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StAccount", accountId);
            }
            else
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);

            esq.Filters.Add(esqFirstFilter);


            if (requestId != null && requestId!="")
            {
                esqSecondFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StSequenceNumber", requestId);
                esq.Filters.Add(esqSecondFilter);
            }

            if (from != null && from != "")
            {
                fromDate = Convert.ToDateTime(from);
                esqFromFilter = esq.CreateFilterWithParameters(FilterComparisonType.GreaterOrEqual, "CreatedOn", fromDate);
                esq.Filters.Add(esqFromFilter);
            }

            

            if (to != null && to != "")
            {
                toDate = Convert.ToDateTime(to);
                esqToFilter = esq.CreateFilterWithParameters(FilterComparisonType.LessOrEqual, "CreatedOn", toDate);
                esq.Filters.Add(esqToFilter);
            }

            



            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                int value = -1;
                try
                {
                    int.TryParse((string)entity.GetColumnValue("StRequestType"), out value);
                }
                catch { }

                if (value >= 0)
                {
                    DateTime CreatedOn = entity.GetTypedColumnValue<DateTime>("CreatedOn");
                    requests.Add(new RequestHeaderDetail()
                    {
                        step = entity.GetTypedColumnValue<string>(stagecol.Name), //(string)entity.GetColumnValue("StStep"),
                        technicalStep = entity.GetTypedColumnValue<string>(wostagecol.Name),//(string)entity.GetColumnValue("StTechnicalStep"),
                        RequestId = (Guid)entity.GetColumnValue("StRequestId"),
                        status = (string)entity.GetColumnValue("StStatusName"),
                        contractId = (string)entity.GetColumnValue("StContractID"),
                        CreatedOn = CreatedOn.ToString("dd/MM/yyyy"),
                        CreatedBy = entity.GetTypedColumnValue<string>(createdbycol.Name),
                        SequenceNumber = (string)entity.GetColumnValue("StSequenceNumber"),
                        RequestTypeName = Utilities.GetEnumAttribute<OperationType, DescriptionAttribute>((OperationType)value).Description,
                        EntityName = Utilities.GetEnumAttribute<OperationType, EntitySchemaNameAttribute>((OperationType)value).schemaName
                    });
                }
                  
            }


            return requests;
        } 
        public string GetEntityNameByRequestId(string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string entityName="";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StRequestId");
            esq.AddColumn("StRequestType");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                int value = int.Parse((string)entity.GetColumnValue("StRequestType"));
                entityName = Utilities.GetEnumAttribute<OperationType, EntitySchemaNameAttribute>((OperationType)value).schemaName;

            }

            return entityName;

        }

        


        public string GetSchemaName(string requestId, string entityName)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<RequestHeaderDetail> requests = new List<RequestHeaderDetail>();
            string editpage = null;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysModule");
            esq.RowCount = 1;
            esq.AddColumn("Id");
            esq.AddColumn("Attribute");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Code", entityName);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                string stepColumnName = (string)entities[0].GetColumnValue("Attribute");

                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
                esq.RowCount = 1;
                esq.AddColumn("Id");
                esq.AddColumn(stepColumnName);

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
                esq.Filters.Add(esqFirstFilter);

                entities = esq.GetEntityCollection(BPM_UserConnection);

                if (entities.Count > 0)
                {
                    var typeColumnValue = entities[0].GetColumnValue(stepColumnName+"Id");

                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysModuleEdit");
                    esq.RowCount = 1;
                    esq.AddColumn("Id");
                    esq.AddColumn("ActionKindName");

                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "TypeColumnValue", typeColumnValue);
                    esq.Filters.Add(esqFirstFilter);

                    entities = esq.GetEntityCollection(BPM_UserConnection);

                    if (entities.Count > 0)
                    {
                         editpage = (string)entities[0].GetColumnValue("ActionKindName");

                    }

                }
          }
            return editpage;

        }

        public void CustomerCreationProcess(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            //Get infos from contact table in CRM database 

            if (contactId != null && contactId != "")
            {
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("Name");
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomersCategory");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryId = entities[0].GetColumnValue("StCustomersCategoryId");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomersCategoryStName");


                    // call SOM to Create Cutomer
                }

            }
            else
            {
                //account
            }
        }

        public void UpdateCustomerData(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            string customerLanguage ="";
            string nationalityNumber = "";
            string customerTitle = "";
            string documentIdType = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StUpdateCustomerAddress");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StCustomerNewTitle");
            esq.AddColumn("StCustomerNewTitle.Id");
            esq.AddColumn("StFirstName");
            esq.AddColumn("StLastName");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StStreet");
            esq.AddColumn("StFloorNumber");
            esq.AddColumn("StMiddleName");          
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");;
            esq.AddColumn("StBuildingNumber");

            esq.AddColumn("StAddressNotes");
            esq.AddColumn("StBirthDate");
            esq.AddColumn("StBusinessPhone");
            esq.AddColumn("StCareer");
            esq.AddColumn("StDocumentIDType");
            esq.AddColumn("StDocumentIDType.Id");
            esq.AddColumn("StEmail");
            esq.AddColumn("StFaxNumber");
            esq.AddColumn("StHomePhone");
            esq.AddColumn("StLanguage");
            esq.AddColumn("StLanguage.Id");
            esq.AddColumn("StMobilePhone");
            esq.AddColumn("StMotherName");
            esq.AddColumn("StNationalID");
            esq.AddColumn("StNationality");
            esq.AddColumn("StNationality.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var lastName = entities[0].GetColumnValue("StLastName");
                var firstName = entities[0].GetColumnValue("StFirstName");
                var middleName = entities[0].GetColumnValue("StMiddleName");
                var addressId = entities[0].GetColumnValue("StAddressID");  
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var street = entities[0].GetColumnValue("StStreet");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloorNumber");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");

                var notes = entities[0].GetColumnValue("StAddressNotes");

                DateTime birthDate = entities[0].GetTypedColumnValue<DateTime>("StBirthDate");
                var businessPhone = entities[0].GetColumnValue("StBusinessPhone");
                var career = entities[0].GetColumnValue("StCareerName");
                var documentIDType = entities[0].GetColumnValue("StDocumentIDTypeId");
                var email = entities[0].GetColumnValue("StEmail");
                var faxNumber = entities[0].GetColumnValue("StFaxNumber");
                var homePhone = entities[0].GetColumnValue("StHomePhone");
                var languageId = entities[0].GetColumnValue("StLanguageId");
                var mobilePhone = entities[0].GetColumnValue("StMobilePhone");
                var motherName = entities[0].GetColumnValue("StMotherName");
                var nationalID = entities[0].GetColumnValue("StNationalID");
                var nationalityId = entities[0].GetColumnValue("StNationalityId");
                var titleId = entities[0].GetColumnValue("StCustomerNewTitleId");
                var town = entities[0].GetColumnValue("StTownName");

                if (!string.IsNullOrEmpty(nationalityId.ToString()))
                {
                     nationalityNumber = GetNationalityNumber(nationalityId.ToString());
                }

                if (!string.IsNullOrEmpty(languageId.ToString()))
                {
                    customerLanguage = GetCustomerLanguage(languageId.ToString());
                }

                if (!string.IsNullOrEmpty(titleId.ToString()))
                {
                    customerTitle = GetCustomerTitle(titleId.ToString());
                }

                if (!string.IsNullOrEmpty(documentIDType.ToString()))
                {
                    documentIdType = GetDocumentType(documentIDType.ToString());
                }


                


                SOMRequestInput<CustomerAddressInput> somRequestInput = new SOMRequestInput<CustomerAddressInput>
                {
                    InputArguments = new CustomerAddressInput
                    {
                        City = city.ToString(),
                        Building = building.ToString(),
                        Floor = floor.ToString(),
                        MiddleName = middleName.ToString(),
                        StateProvince = province.ToString(),
                        Region = area.ToString(),
                        FirstName = firstName.ToString(),
                        LastName = lastName.ToString(),
                        AddressSeq = long.Parse(addressId.ToString()),
                        Street = street.ToString(),
                        AddressNotes = notes.ToString(),
                        Birthdate = birthDate.ToString("MM/dd/yyyy"),
                        BusinessPhone = businessPhone.ToString(),
                        Career = career.ToString(),
                        DocumentIdType = documentIdType.ToString(),
                        Email = email.ToString(),
                        FaxNumber = faxNumber.ToString(),
                        HomePhone = homePhone.ToString(),
                        Language = customerLanguage,
                        MobilePhone = mobilePhone.ToString(),
                        MotherName = motherName.ToString(),
                        NationalId = nationalID.ToString(),
                        Nationality= nationalityNumber,
                        Title= customerTitle,
                        Town= town.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<CustomerAddressInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomerAddress/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }

        }

        public void UpdateAccountEnterprise(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            string nationalityNumber = "";
            string documentIdType = "";
            string customerTitle = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StUpdateAccount");
         
            esq.AddColumn("StPrimaryContact");
            esq.AddColumn("StPrimaryContact.Id");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCompanyId");
            esq.AddColumn("StBranch");
            esq.AddColumn("StCompanyName");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StStreet");
            esq.AddColumn("StFloorNumber");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StNationalityLookup");
            esq.AddColumn("StNationalityLookup.Id");
            esq.AddColumn("StBusinessTypeLookup");
            esq.AddColumn("StBusinessTypeLookup.Id");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StMobilePhone");
            esq.AddColumn("StBusinessPhone");
            esq.AddColumn("StFaxNumber");
            esq.AddColumn("StEmail");
            esq.AddColumn("StAddressNotes");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var addressId = entities[0].GetColumnValue("StAddressID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var companyName = entities[0].GetColumnValue("StCompanyName");
                var branch = entities[0].GetColumnValue("StBranch");
                var street = entities[0].GetColumnValue("StStreet");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloorNumber");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var primaryContactId = entities[0].GetColumnValue("StPrimaryContactId");
                var compnayId = entities[0].GetColumnValue("StCompanyId");
                var businessType = entities[0].GetColumnValue("StBusinessTypeLookupName");

                var notes = entities[0].GetColumnValue("StAddressNotes");

                var businessPhone = entities[0].GetColumnValue("StBusinessPhone");
                var email = entities[0].GetColumnValue("StEmail");
                var faxNumber = entities[0].GetColumnValue("StFaxNumber");
                var mobilePhone = entities[0].GetColumnValue("StMobilePhone");
                var nationalityId = entities[0].GetColumnValue("StNationalityLookupId");
                var town = entities[0].GetColumnValue("StTownName");

                Contact contact = new CommonManager().GetContact(primaryContactId.ToString());

                if (!string.IsNullOrEmpty(nationalityId.ToString()))
                {
                    nationalityNumber = GetNationalityNumber(nationalityId.ToString());
                }

                if (!string.IsNullOrEmpty(contact.Title.ToString()))
                {
                    customerTitle = GetCustomerTitle(contact.Title.ToString());
                }

                if (!string.IsNullOrEmpty(contact.DocumentIdTypeId.ToString()))
                {
                    documentIdType = GetDocumentType(contact.DocumentIdTypeId.ToString());
                }



                SOMRequestInput<UpdateEnterpriseAccountInput> somRequestInput = new SOMRequestInput<UpdateEnterpriseAccountInput>
                {
                    InputArguments = new UpdateEnterpriseAccountInput
                    {
                        CompanyId = compnayId.ToString(),
                        CompanyName = companyName.ToString(),
                        Branch = branch.ToString(),
                        Title = customerTitle,
                        AddressNotes = notes.ToString(),
                        BusinessPhone =businessPhone.ToString(),
                        BusinessType = businessType.ToString(),
                        DocumentIdType = documentIdType,
                        Email =email.ToString(),
                        FaxNumber =faxNumber.ToString(),
                        MainContactId = contact.CustomerId.ToString(),
                        MobilePhone = mobilePhone.ToString(),
                        NationalId = contact.DocumentID,
                        Nationality = nationalityNumber,
                        City = city.ToString(),
                        Building = building.ToString(),
                        Floor = floor.ToString(),
                        MiddleName = contact.MiddleName.ToString(),
                        StateProvince = province.ToString(),
                        Region = area.ToString(),
                        FirstName = contact.GivenName.ToString(),
                        LastName = contact.SurName.ToString(),
                        AddressSeq = long.Parse(addressId.ToString()),
                        Street = street.ToString(),
                        Town = town.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<UpdateEnterpriseAccountInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateAccountCustomerAddress/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }

        }

        public void UpdateAccounOfficial(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            string nationalityNumber = "";
            string documentIdType = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StUpdateAccountOfficial");
            esq.AddColumn("StPrimaryContact");
            esq.AddColumn("StPrimaryContact.Id");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCompanyName");
            esq.AddColumn("StMinistry");
            esq.AddColumn("StEndCustomerName");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StStreet");
            esq.AddColumn("StFloorNumber");
            esq.AddColumn("StAddressID");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StNationalityLookup");
            esq.AddColumn("StNationalityLookup.Id");
            esq.AddColumn("StBusinessTypeLookup");
            esq.AddColumn("StBusinessTypeLookup.Id");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StMobilePhone");
            esq.AddColumn("StBusinessPhone");
            esq.AddColumn("StFaxNumber");
            esq.AddColumn("StEmail");
            esq.AddColumn("StAddressNotes");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var addressId = entities[0].GetColumnValue("StAddressID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var ministry = entities[0].GetColumnValue("StMinistry");
                var endCustomerName = entities[0].GetColumnValue("StEndCustomerName");
                var street = entities[0].GetColumnValue("StStreet");
                var building = entities[0].GetColumnValue("StBuildingNumber");
                var floor = entities[0].GetColumnValue("StFloorNumber");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var primaryContactId = entities[0].GetColumnValue("StPrimaryContactId");
                var compnayName = entities[0].GetColumnValue("StCompanyName");
                var businessType = entities[0].GetColumnValue("StBusinessTypeLookupName");

                var notes = entities[0].GetColumnValue("StAddressNotes");

                var businessPhone = entities[0].GetColumnValue("StBusinessPhone");
                var email = entities[0].GetColumnValue("StEmail");
                var faxNumber = entities[0].GetColumnValue("StFaxNumber");
                var mobilePhone = entities[0].GetColumnValue("StMobilePhone");
                var nationalityId = entities[0].GetColumnValue("StNationalityLookupId");
                var town = entities[0].GetColumnValue("StTownName");

                Contact contact = new CommonManager().GetContact(primaryContactId.ToString());

                if (!string.IsNullOrEmpty(nationalityId.ToString()))
                {
                    nationalityNumber = GetNationalityNumber(nationalityId.ToString());
                }


                if (!string.IsNullOrEmpty(contact.DocumentIdTypeId.ToString()))
                {
                    documentIdType = GetDocumentType(contact.DocumentIdTypeId.ToString());
                }



                SOMRequestInput<UpdateOfficialAccountInput> somRequestInput = new SOMRequestInput<UpdateOfficialAccountInput>
                {
                    InputArguments = new UpdateOfficialAccountInput
                    {
                        Name = compnayName.ToString(),
                        MinistryName = ministry.ToString(),
                        EndCustomerName = endCustomerName.ToString(),
                        AddressNotes = notes.ToString(),
                        BusinessPhone = businessPhone.ToString(),
                        BusinessType = businessType.ToString(),
                        DocumentIdType = documentIdType,
                        Email = email.ToString(),
                        FaxNumber = faxNumber.ToString(),
                        MainContactId = contact.CustomerId.ToString(),
                        MobilePhone = mobilePhone.ToString(),
                        NationalId = contact.DocumentID,
                        Nationality = nationalityNumber,
                        City = city.ToString(),
                        Building = building.ToString(),
                        Floor = floor.ToString(),
                        StateProvince = province.ToString(),
                        Region = area.ToString(),
                        AddressSeq = long.Parse(addressId.ToString()),
                        Street = street.ToString(),
                        Town = town.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<UpdateOfficialAccountInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateOfficialCustomerAddress/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }

        }

        public void UpdatePaymentArrangement(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StUpdatePaymentArrangement");
            esq.AddColumn("StPaymentMethodID");
            esq.AddColumn("StBankLookup");
            esq.AddColumn("StBankLookup.Id");
            esq.AddColumn("StAccountOwner");
            esq.AddColumn("StAccountNumber");
            esq.AddColumn("StDebitCreditCard");
            esq.AddColumn("StCustomerId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var paymentMethodId = entities[0].GetColumnValue("StPaymentMethodID");
                var bankId = entities[0].GetColumnValue("StBankLookupId");
                var accountOwner = entities[0].GetColumnValue("StAccountOwner");
                var accountNumber = entities[0].GetColumnValue("StAccountNumber");
                var debitCreditCard = entities[0].GetColumnValue("StDebitCreditCard");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                CustomerPaymentProfileInput customerPaymentProfileInput = new CustomerPaymentProfileInput {
                    AccountNumber = accountNumber.ToString(),
                    AccountOwner = accountOwner.ToString(),
                    DebitCreditCard = debitCreditCard.ToString(),
                    PaymentMethodId = paymentMethodId.ToString(),
                    CommonInputArgument = new CommonInputArgument()
                    {
                        CustomerId = customerId.ToString(),
                        RequestId = requestId.ToString()
                    }
                };
                if (!string.IsNullOrEmpty(bankId.ToString()))
                { Bank bank = GetBank(bankId.ToString());
                    customerPaymentProfileInput.BankCode = bank.Code;
                    customerPaymentProfileInput.BankAddress = bank.Address;
                    customerPaymentProfileInput.BankName = bank.Name;
                }

                SOMRequestInput<CustomerPaymentProfileInput> somRequestInput = new SOMRequestInput<CustomerPaymentProfileInput>
                {
                    InputArguments = customerPaymentProfileInput
                };

                using (var client = new SOMClient())
                {
                    output= client.Post<SOMRequestInput<CustomerPaymentProfileInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomerPaymentProfile/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);
            }

        }

        public void UpdateCustomerCategory(string customerCategoryId, string customerId, string requestId)
        {
            var output = new SOMRequestOutput();
                SOMRequestInput<CustomerCategoryInput> somRequestInput = new SOMRequestInput<CustomerCategoryInput>
                {
                    InputArguments = new CustomerCategoryInput
                    {
                        CustomerCategoryId = customerCategoryId,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId,
                            RequestId = requestId
                        }
                    }
                };
           
            using (var client = new SOMClient())
            {
                output = client.Post<SOMRequestInput<CustomerCategoryInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomer/StartProcess", somRequestInput);
            }

            var manager = new BusinessEntityManager();
            manager.InsertSOMRequestToProcessInstancesLogs(Guid.Parse(requestId), output);


        }

        public string UpdateCustomerCategoryByBSCS(string id, string bscsId)
        {
            var result = "";
            try
            {
                string entity = "Contact";
                string Attribute = "StCustomerCategoryID";
                string columncheck = "StCustomerId";

                var customerName = GetCustomerCategoryNameById(bscsId);
                UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var update = new Update(connection, entity).Set(Attribute, Column.Parameter(bscsId))
                    .Where(columncheck).IsEqual(Column.Parameter(id));
                update.Execute();

                var update2 = new Update(connection, entity).Set("StCustomerCategoryName", Column.Parameter(customerName))
                    .Where(columncheck).IsEqual(Column.Parameter(id));
                update2.Execute();
            }
            catch { }
            return result;
        }

        public string GetCustomerCategoryNameById(string customerCategoryId )
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            string customerCategoryName = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");

            esq.AddColumn("StCustomerCategoryName");
            esq.AddColumn("StCustomerCategoryID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryID", customerCategoryId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                customerCategoryName = (string)entities[0].GetColumnValue("StCustomerCategoryName");
            }
            return customerCategoryName;
        }


        public void CustomerCreation(string CustomerCategoryId, string PaymentMethodId, string City, string FirstName, string LastName, string CustomerId, string CSO, string BankCode, string AccountNumber, string contactId, string accountId)
        {

            SOMRequestInput<CustomerCreationInput> somRequestInput = new SOMRequestInput<CustomerCreationInput>
            {
                InputArguments = new CustomerCreationInput
                {
                    AccountNumber = AccountNumber,
                    BankCode = BankCode,
                    City = City,
                    CSO = CSO,
                    CustomerCategoryId = CustomerCategoryId,
                    FirstName = FirstName,
                    LastName = LastName,
                    PaymentMethodId = PaymentMethodId,
                    CommonInputArgument = new CommonInputArgument()
                    {
                        ContactId = contactId,
                        CustomerId = CustomerId
                    }
                }

            };

            using (var client = new SOMClient())
            {
                client.Post<SOMRequestInput<CustomerCreationInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateCustomer/StartProcess", somRequestInput);
            }

        }
        public CustomerCreationOutput CreateCustomerFromIndividual (string contactId,string CustomerCategoryId, string segmentId, string CSO, string paymentMethodId)
        {

            CommonManager commonManager = new CommonManager();
            var contact = commonManager.GetContact(contactId);
            if (contact != null)
                return CreateCustomer(CustomerCategoryId,paymentMethodId,contact.CityId,contact.GivenName,contact.SurName,contact.CSOId,contact.BankID,contact.BankAccountID,contact.BankName,contact.IBAN
                    ,contact.CountryId,"DefaultRatePlan",contact.NationalityId,contact.BirthDate,contact.BuildingNumber,contact.Career,contact.DocumentID,contact.DocumentIdTypeId,contact.Email,
                    contact.FaxNumber, contact.FloorNumber, contact.HomePhone,contact.MiddleName,contact.MobilePhone,contact.MotherName,contact.DistrictId,contact.Street,contact.RegionId,contact.LanguageId
                    ,contact.Title,contact.TownId,contact.AddressNotes,contact.BusinessPhone,segmentId,"","");
            else return null;
            
        }
        public CustomerCreationOutput CreateCustomer(string CustomerCategoryId, string PaymentMethodId, string City, string FirstName, string LastName, string CSO,
            string BankCode, string AccountNumber, string BankName, string IBAN, string country, string DefaultRatePlan, string nationality, string birthDate, string building, string career,
            string documentId, string documentTypeId, string email, string faxNumber, string floor, string homePhone, string middleName, string mobilePhone, string motherName, string region,
            string street, string stateProvince, string language, string title, string town, string addressNotes, string businessPhone, string segmentId,string accountOwner,string debitCreditCard
            )
        {
            IDManager manager = new IDManager();
            string customerId = manager.GetCustomerNextId();
            DateTime dob = DateTime.MinValue;
            DateTime.TryParse(birthDate, out dob);
            string bankName = "";
            string bankAddress = "";
            if (!string.IsNullOrEmpty(BankCode))
            {
                Bank bank = this.GetBank(BankCode);
                bankName = bank.Name;
                bankAddress = bank.Address;
                BankCode = bank.Code;
            }
            else
                BankCode = "";
            string cso = GetCSOId(CSO);
            string billcycle = GetDefaultBillCycle();
            string externalCustomerId = new CatalogManager().GetExternalCustomerSetId(segmentId);
            string rateplan = GetDefaultRatePlan();
            string countryNumber = GetCountryNumber(country);
            string documentType = GetDocumentType(documentTypeId);
            string nationalityNumber = GetNationalityNumber(nationality);
            string customerTitle = "";
            if(title != "")
            {
                customerTitle = GetCustomerTitle(title);
            }

            string  customerLanguage = "";
            if(language !="")
            {
                customerLanguage = GetCustomerLanguage(language);
            }
            CustomerCreationOutput output = new CustomerCreationOutput();
            CreateCustomerInput input = new CreateCustomerInput
            {
                CustomerId = customerId,
                CustomerCategoryId = CustomerCategoryId,
                DefaultRatePlan = rateplan,
                Title = customerTitle,
                FirstName = FirstName,
                MiddleName = middleName,
                LastName = LastName,
                MotherName = motherName,
                BirthDate = dob,
                Career = career,
                Language = customerLanguage,
                Nationality = nationalityNumber,
                ExternalCustomerSetId = externalCustomerId,
                Country = countryNumber,
                StateProvince = stateProvince, // Region in UI
                City = City,
                Town = town,
                Region = region, //District in UI
                Street = street,
                Building = building,
                Floor = floor,
                AddressNotes = addressNotes,
                HomePhone = homePhone,
                FaxNumber = faxNumber,
                MobilePhone = mobilePhone,
                BusinessPhone = businessPhone,
                Email = email,
                CSO = cso,
                PaymentResponsibility = true,
                PaymentMethodId = PaymentMethodId,
                BillCycle = billcycle,
                BankCode = BankCode, // BankID in UI
                BankName = bankName,
                AccountNumber = AccountNumber, //BankAccountID
                DebitAccountOwner = FirstName,
                IBAN = IBAN,
                BankSwiftCode = "",
                DocumentId = documentId,
                DocumentTypeId = documentType,
                ValidFromDate = DateTime.Now,
                AccountOwner=accountOwner,
                DebitCreditCard=debitCreditCard,
                BankAddress= bankAddress
            };

            using (var client = new SOMClient())
            {
                output = client.Post<CreateCustomerInput, CustomerCreationOutput>("api/SOM.ST/Billing/CreateCustomer", input);
                output.CustomerSequenceId = customerId;
            }
            return output;
        }

        public CustomerCreationOutput CreateAccount(string contactId ,string parentCustomerId, string CustomerCategoryId, string levelId, string companyName, string branch,string companyId,string FirstName, string LastName, string CSO,
         string BankCode, string AccountNumber, string BankName, string IBAN, string country, string nationality, string birthDate, string building, string career,string City,
         string documentId, string documentTypeId, string email, string faxNumber, string floor, string homePhone, string middleName, string mobilePhone, string motherName, string region,
         string street, string stateProvince, string language, string title, string town, string addressNotes, string businessPhone, string segmentId, string PaymentMethodId,string businessType,string accountOwner,string debitCreditCard
         )
        {
            IDManager manager = new IDManager();
            CommonManager commonManager = new CommonManager();
            Contact contact = commonManager.GetContact(contactId);
            string customerId = manager.GetCustomerNextId();
            DateTime dob = DateTime.MinValue;
            DateTime.TryParse(birthDate, out dob);
            string bankName = "";
            string bankAddress = "";
            if (!string.IsNullOrEmpty(BankCode))
            {
                Bank bank = this.GetBank(BankCode);
                bankName = bank.Name;
                bankAddress = bank.Address;
                BankCode = bank.Code;
            }
            else
                BankCode = "";
            string cso = GetCSOId(CSO);
            string billcycle = GetDefaultBillCycle();
            string externalCustomerId = new CatalogManager().GetExternalCustomerSetId(segmentId);
            string rateplan = GetDefaultRatePlan();
            string countryNumber = GetCountryNumber(country);
            string documentType = "";// GetDocumentType(documentTypeId);
            string nationalityNumber = GetNationalityNumber(nationality);
            string customerTitle = "";
            if (contact.Title != "")
            {
                customerTitle = GetCustomerTitle(contact.Title);
            }

            string customerLanguage = "";
            if (language != "")
            {
                customerLanguage = GetCustomerLanguage(language);
            }
            CustomerCreationOutput output = new CustomerCreationOutput();
            CreateLargeMemberCustomerInput input = new CreateLargeMemberCustomerInput
            {
                ParentCustomerId = parentCustomerId,
                MainContactId=contact.CustomerId,
                CustomerId = customerId,
                CustomerCategoryId = CustomerCategoryId,
                LevelId = levelId,
				CompanyName= companyName,
				Branch= branch,
				CompanyId= companyId,
                DefaultRatePlan = rateplan,
                Title = customerTitle,
                FirstName = contact.GivenName,
                MiddleName = contact.MiddleName,
                LastName = contact.SurName,
                Nationality = nationalityNumber,
                ExternalCustomerSetId = externalCustomerId,
                Country = countryNumber,
                StateProvince = stateProvince,
                City = City,
                Town = town,
                Region = region,
                Street = street,
                Building = building,
                Floor = floor,
                AddressNotes = addressNotes,
                FaxNumber = faxNumber,
                MobilePhone = mobilePhone,
                BusinessPhone = businessPhone,
                Email = email,
                CSO = cso,
                //ContractResponsibity = true,
                PaymentResponsibility = true,
                PaymentMethodId = PaymentMethodId,
                BillCycle = billcycle,
                BankCode =BankCode,
                BankName = bankName,
                AccountNumber = AccountNumber,
                DebitAccountOwner = contact.GivenName,
                IBAN = IBAN,
                BankSwiftCode = "",
                DocumentId = contact.DocumentID,
                DocumentTypeId = GetDocumentType(contact.DocumentIdTypeId),
                ValidFromDate = DateTime.Now,
                BusinessType= businessType,
                BankAddress= bankAddress,
                DebitCreditCard=debitCreditCard,
                AccountOwner=accountOwner
            };

            using (var client = new SOMClient())
            {
                output = client.Post<CreateLargeMemberCustomerInput, CustomerCreationOutput>("api/SOM.ST/Billing/CreateLargeCustomer", input);
                output.CustomerSequenceId = customerId;
            }
            return output;
        }

        public CustomerCreationOutput CreateOfficialAccount(OfficialAccountInput officialAccountInput) {
            CustomerCreationOutput output = new CustomerCreationOutput();
            IDManager manager = new IDManager();
            CommonManager commonManager = new CommonManager();
            Contact contact = commonManager.GetContact(officialAccountInput.ContactId);
            if (!string.IsNullOrEmpty(officialAccountInput.BankCode))
            {
                Bank bank = this.GetBank(officialAccountInput.BankCode);
                officialAccountInput.BankAddress = bank.Address;
                officialAccountInput.BankName = bank.Name;
                officialAccountInput.DebitAccountOwner = contact.GivenName;
                officialAccountInput.BankCode = bank.Code;
            }
            else
                officialAccountInput.BankCode = "";
            officialAccountInput.CustomerId = manager.GetCustomerNextId();
            officialAccountInput.MainContactId = contact.CustomerId;
            officialAccountInput.DocumentId = contact.DocumentID;
            officialAccountInput.DocumentIdType = GetDocumentType(contact.DocumentIdTypeId);
            officialAccountInput.PaymentResponsibility = true;
            officialAccountInput.CSO = GetCSOId(officialAccountInput.CSO);
            officialAccountInput.BillCycle = GetDefaultBillCycle();
            officialAccountInput.ExternalCustomerSetId = new CatalogManager().GetExternalCustomerSetId(officialAccountInput.ExternalCustomerSetId);
            officialAccountInput.DefaultRatePlan = GetDefaultRatePlan();
            officialAccountInput.Country = GetCountryNumber(officialAccountInput.Country);
            officialAccountInput.Nationality = GetNationalityNumber(officialAccountInput.Nationality);

            using (var client = new SOMClient())
            {
                output = client.Post<OfficialAccountInput, CustomerCreationOutput>("api/SOM.ST/Billing/CreateOfficialCustomer", officialAccountInput);
                output.CustomerSequenceId = officialAccountInput.CustomerId;
            }
            return output;
        }

        public Bank GetBank(string id)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            Bank bank = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StBank");
            esq.AddColumn("StName");
            esq.AddColumn("StCode");
            esq.AddColumn("StAddress");
            esq.AddColumn("StDescription");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", id);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var name = entities[0].GetColumnValue("StName");
                var code = entities[0].GetColumnValue("StCode");
                var address = entities[0].GetColumnValue("StAddress");
                var description = entities[0].GetColumnValue("StDescription");

                bank = new Bank()
                {
                    Name = name.ToString(),
                    Code = code.ToString(),
                    Description = address.ToString(),
                    Address = description.ToString(),
                };
            }
            return bank;
        }
        public string GetCSOId(string Id)
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCSO");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StCSOBSCSId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetDefaultBillCycle()
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StBillingCycleId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "92E58A70-8D14-4DC6-ABB4-EA10409D91B4"));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetDefaultRatePlan()
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StRatePlanId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "92E58A70-8D14-4DC6-ABB4-EA10409D91B4"));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetCustomerTitle(string Id)
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerTitle");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StBSCSId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetDocumentType(string Id)
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDocumentIdType");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StBSCSId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetCustomerLanguage(string Id)
        {

            string number = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerLanguage");
            esq.AddColumn("Id");
            var numberCol = esq.AddColumn("StBSCSId");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                number = entities[0].GetTypedColumnValue<string>(numberCol.Name);
            }
            return number;
        }
        public string GetCountryNumber(string Id)
        {

            string countrynumber = "" ;
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Country");
            esq.AddColumn("Id");
            var countryNumberCol = esq.AddColumn("StCountryNumber");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                countrynumber = entities[0].GetTypedColumnValue<string>(countryNumberCol.Name);
            }
            return countrynumber;
        }
        public string GetNationalityNumber(string Id)
        {

            string countrynumber = "";
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StNationalityLookup");
            esq.AddColumn("Id");
            var countryNumberCol = esq.AddColumn("StCountryNumber");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", Id));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                countrynumber = entities[0].GetTypedColumnValue<string>(countryNumberCol.Name);
            }
            return countrynumber;
        }
        public List<CustomerCategoryInfo> GetCustomerCategoriesInfoBySegmentId(string segmentId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<CustomerCategoryInfo> customerCategoryItems = new List<CustomerCategoryInfo>();
            //Guid id = new Guid(segmentId.ToUpper());

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoryCatalog");
            var IdCol = esq.AddColumn("Id");
            esq.AddColumn("StName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerSegments.Id", segmentId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                Guid catalogId = entities[0].GetTypedColumnValue<Guid>(IdCol.Name);//GetColumnValue("Id");

                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
                esq.AddColumn("Id");
                esq.AddColumn("StCustomerCategoryName");
                esq.AddColumn("StCustomerCategoryID");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryCatalog.Id", catalogId);
                esq.Filters.Add(esqFirstFilter);

                entities = esq.GetEntityCollection(BPM_UserConnection);

                for (int i = 0; i < entities.Count; i++)
                {
                    var name = entities[i].GetColumnValue("StCustomerCategoryName");
                    var customerCategoryId = entities[i].GetColumnValue("StCustomerCategoryID");

                    var customerCategoryItem = new CustomerCategoryInfo()
                    {
                        CategoryId = customerCategoryId.ToString(),
                        Name = name.ToString()
                    };
                    customerCategoryItems.Add(customerCategoryItem);
                }
            }
            return customerCategoryItems;
        }


        public SOMRequestOutput CreateTelephonyContractOnHold(Guid requestId, string coreServices, string optionalServices , string ratePlanId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output  =new SOMRequestOutput();
            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            List<DepositDocument> depositServices = new List<DepositDocument>();
            string linePathId,serviceResourceId="";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StNumberToReserve");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StAddressNotes");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StCountry");
            esq.AddColumn("StCountry.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");
            esq.AddColumn("StSubTypes");
            esq.AddColumn("StSubTypes.Id");
            var subType = esq.AddColumn("StSubTypes.StName");
            esq.AddColumn("StSponsor");
            var sponsorcol = esq.AddColumn("StSponsor.Id");



            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var phoneNumber = entities[0].GetColumnValue("StNumberToReserve");
                var addressNotes = entities[0].GetColumnValue("StAddressNotes");
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                string pathId = entities[0].GetColumnValue("StLinePathID").ToString();
                var subTypeId = entities[0].GetColumnValue("StSubTypesId");
                var countryId = entities[0].GetColumnValue("StCountryId");
                var city = entities[0].GetColumnValue("StCityName");
                    var area = entities[0].GetColumnValue("StAreaName");
                    var province = entities[0].GetColumnValue("StProvinceName");
                    var town = entities[0].GetColumnValue("StTownName");
                var locationType = entities[0].GetColumnValue("StLocationName");
                var subTypeName = entities[0].GetTypedColumnValue<string>(subType.Name);


                CRMCustomerInfo info = contactId != null ? GetCRMCustomerInfo(contactId.ToString(), null) : GetCRMCustomerInfo(null, accountId.ToString());

                if (coreServices != "\"\"") listOfCoreServices= JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);

                }


                if (pathId.EndsWith(".0")) {
                            linePathId = pathId.Substring(0, pathId.Length - 2);
                }
                else
                {
                    linePathId = pathId;
                }

                Guid sponsor = Guid.Empty;
                try {
                    sponsor = entities[0].GetTypedColumnValue<Guid>(sponsorcol.Name); }
                catch { }

                if(contactId != null) { 
                    bool isForeigner = IsContactForeigner(new Guid(contactId.ToString()));
                    if(isForeigner && ( sponsor == Guid.Empty))
                    {
                        depositServices = new CatalogManager().GetForeignerDeposits(optionalServices);
                    }
                }
                /* depositServices = (from item in listOfOptionalServices
                                    where item.HasDeposit
                                    select new DepositDocument() { Id = item.Id }).ToList();*/


                //call api
                SOMRequestInput<TelephonyContractOnHoldInput> somRequestInput = new SOMRequestInput<TelephonyContractOnHoldInput>
                {
                    InputArguments = new TelephonyContractOnHoldInput
                    {
                        LinePathId = linePathId,
                        PhoneNumber = phoneNumber.ToString(),
                        SubType = subTypeName,//new CommonManager().GetSubTypeIdentifier(subTypeId.ToString()),
                        ServiceResource = serviceResourceId,
                        City = city.ToString(),
                        Building = buildingNumber.ToString(),
                        Floor = floor.ToString(),
                        Town = town.ToString(),
                        StateProvince = province.ToString(),
                        Street= street.ToString(),
                        Region = area.ToString(),
                        CountryId = GetCountryNumber(countryId.ToString()),
                        LocationType = locationType.ToString(),
                        Notes = addressNotes.ToString(),
                        CSO = info.csoBSCSId,
                        RatePlanId = ratePlanId,
                        ContractServices = contractServices,
                        DepositServices = depositServices,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContactId = contactId!=null? contactId.ToString() : null,
                            AccountId = accountId != null ? accountId.ToString() : null,
                            RequestId = requestId.ToString(),
                            CustomerId = info.CustomerId 
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<TelephonyContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateTelephonyContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

            
            string stringifiedProcessId = output.ProcessId;
            if (output.ProcessId != null)
            {
                long processId;
                long.TryParse(stringifiedProcessId,out processId);
                var businessEntityManager = new BusinessEntityManager();
                businessEntityManager.InsertInstanceToProcessInstancesLogs(requestId, processId);
            }

            return output;

            //return new SOMRequestOutput()
            //{
            //    ProcessId = "10178"
            //};

        }





        public int CheckTelephonyContractOnHoldStatus(string processId)
        {
            int status;

            using (SOMClient client = new SOMClient())
            {
                status = client.Get<int>(String.Format("api/SOM.ST/Common/CheckWorkflowStatus?processInstanceId={0}", processId));
            }

            return status;

            //Array values = Enum.GetValues(typeof(ContacrtOnHoldStatus));
            //Random random = new Random();
            //int randomBar = (int)values.GetValue(random.Next(values.Length));
            //return randomBar;

        }

        public bool IsContactForeigner(Guid contactId)
        {
            
            bool isForeigner = false;
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Id");
            var nationalitycol = esq.AddColumn("StCustomerDocumentType.Id");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isForeigner = entities[0].GetTypedColumnValue<Guid>(nationalitycol.Name) == Guid.Parse("39A1AAE9-6FC8-4204-82B7-60E5EEE91E03") ? true : false;
            }
            return isForeigner;
        }

        public void UpdateContactAddressID(string requestId, string addressId)
        {
            var UserConnection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var recordSchema = UserConnection.EntitySchemaManager.GetInstanceByName("Contact");
            var recordEntity = recordSchema.CreateEntity(UserConnection);

            var eSQ = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "Contact");
            eSQ.RowCount = 1;
            eSQ.AddAllSchemaColumns();
            eSQ.Filters.Add(eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId));
            var collection = eSQ.GetEntityCollection(UserConnection);
            if (collection.Count > 0)
            {
                recordEntity = collection[0];
                recordEntity.SetColumnValue("StAddressID", addressId);
            }
            recordEntity.Save();
        }

        //public void convertFileToBinaryCode()
        //{
        //    byte[] file;
        //    using (var stream = new FileStream(@"C:\Users\mohamad.abdallah\Desktop\adsl.docx", FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = new BinaryReader(stream))
        //        {
        //            file = reader.ReadBytes((int)stream.Length);
        //        }
        //    }

        //    UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];

        //    var ins = new Insert(connection)
        //       .Into("ContactFile")
        //       .Set("ContactId", Column.Parameter("E7D397DD-75E4-4E07-850E-23DE2ABEDD0A"))
        //       .Set("Name", Column.Parameter("adsl.docx"))
        //       .Set("Data", Column.Parameter(file))
        //       .Set("TypeId", Column.Parameter("529BC2F8-0EE0-DF11-971B-001D60E938C6"))
        //       .Set("Size", Column.Parameter("10"))
        //       .Set("Version", Column.Parameter("1"));

        //    var affectedRows = ins.Execute();


        //}

        //public void convertExcelToBinaryCode()
        //{
        //    byte[] file;
        //    using (var stream = new FileStream(@"C:\Users\mohamad.abdallah\Desktop\excel.csv", FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = new BinaryReader(stream))
        //        {
        //            file = reader.ReadBytes((int)stream.Length);
        //        }
        //    }

        //    UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];

        //    var ins = new Insert(connection)
        //       .Into("ContactFile")
        //       .Set("ContactId", Column.Parameter("E7D397DD-75E4-4E07-850E-23DE2ABEDD0A"))
        //       .Set("Name", Column.Parameter("excel.csv"))
        //       .Set("Data", Column.Parameter(file))
        //       .Set("TypeId", Column.Parameter("529BC2F8-0EE0-DF11-971B-001D60E938C6"))
        //       .Set("Size", Column.Parameter("10"))
        //       .Set("Version", Column.Parameter("1"));

        //    var affectedRows = ins.Execute();


        //}

        //public void convertImageToBinaryCode()
        //{
        //    byte[] file;
        //    using (var stream = new FileStream(@"C:\Users\mohamad.abdallah\Desktop\p.png", FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = new BinaryReader(stream))
        //        {
        //            file = reader.ReadBytes((int)stream.Length);
        //        }
        //    }

        //    UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];

        //    var ins = new Insert(connection)
        //       .Into("ContactFile")
        //       .Set("ContactId", Column.Parameter("E7D397DD-75E4-4E07-850E-23DE2ABEDD0A"))
        //       .Set("Name", Column.Parameter("p.png"))
        //       .Set("Data", Column.Parameter(file))
        //       .Set("TypeId", Column.Parameter("529BC2F8-0EE0-DF11-971B-001D60E938C6"))
        //       .Set("Size", Column.Parameter("10"))
        //       .Set("Version", Column.Parameter("1"));

        //    var affectedRows = ins.Execute();


        //}

        //public void convertPdfToBinaryCode()
        //{
        //    byte[] file;
        //    using (var stream = new FileStream(@"C:\Users\mohamad.abdallah\Desktop\q.pdf", FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = new BinaryReader(stream))
        //        {
        //            file = reader.ReadBytes((int)stream.Length);
        //        }
        //    }

        //    UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];

        //    var ins = new Insert(connection)
        //       .Into("ContactFile")
        //       .Set("ContactId", Column.Parameter("E7D397DD-75E4-4E07-850E-23DE2ABEDD0A"))
        //       .Set("Name", Column.Parameter("q.pdf"))
        //       .Set("Data", Column.Parameter(file))
        //       .Set("TypeId", Column.Parameter("529BC2F8-0EE0-DF11-971B-001D60E938C6"))
        //       .Set("Size", Column.Parameter("10"))
        //       .Set("Version", Column.Parameter("1"));

        //    var affectedRows = ins.Execute();


        //}

        #endregion

        #region Mappers
        public ContractService ServiceDetailToContractServiceMapper(ServiceDetail item)
        {
            return new ContractService
            {
                sncode = item.Id,
                spcode = item.PackageId
            };
        }

        #endregion
    }

    class Flag
    {
        public string templateId { get; set; }
        public string invoiceId { get; set; }
        public string rate { get; set; }
        public bool isApproval { get; set; }
    }


 

}
