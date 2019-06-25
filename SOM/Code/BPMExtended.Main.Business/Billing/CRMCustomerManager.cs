﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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

        public bool IsCustomerSyrian(string contactId, string accountId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Id");
            var c = esq.AddColumn("Country");
            var country = esq.AddColumn("Country.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var countryId = entities[0].GetColumnValue("CountryId");


                if (countryId.ToString().ToLower() == "F9EB7E62-DADB-4D0C-BC2C-38A9A33995B5".ToLower()) return true;
            }

            return false;
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
                item = client.Get<Customer>(String.Format("api/SOM.ST/Billing/ReadCustomer?CustomerId={0}", customerId));
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
                //return ;
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
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCSO");
                esq.AddColumn("StCSO.Id");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryID");
                esq.AddColumn("StCustomerCategoryName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var csoId = entities[0].GetColumnValue("StCSOId");
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                    customerInfo = new CRMCustomerInfo()
                    {
                        CustomerId = customerId.ToString(),
                        CustomerName = name.ToString(),
                        DocumentID = documentId.ToString(),
                        CustomerCategoryID = customerCategoryId.ToString(),
                        CustomerCategoryName = customerCategoryName.ToString(),
                        csoId = csoId.ToString()


                    };
                }

            }
            else
            {
                //account
            }


            return customerInfo;

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

        //if a contract has a completed  pabx operation
        public bool IsRecordExistAndCompletedInRequestHeader(string contractId, string requestType, string statusId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2, esqFilter3;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", requestType);
            esqFilter3 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus", statusId);

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);
            esq.Filters.Add(esqFilter3);

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

        public bool IsCommercial(string sysUserId)
        {
            bool isCommercial = false;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysUserInRole");
            esq.AddColumn("SysUser");
            var commercialColumn = esq.AddColumn("SysUser.Contact.StCSO.StCommercial");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysUser", sysUserId);
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


        public List<RequestHeaderDetail> GetRequestHeaderData(string contactId, string accountId, string contractId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            List<RequestHeaderDetail> requests = new List<RequestHeaderDetail>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StStep");
            esq.AddColumn("StTechnicalStep");
            esq.AddColumn("StRequestId");
            esq.AddColumn("StStatus");
            esq.AddColumn("StContractID");
            esq.AddColumn("StRequestType");
            esq.AddColumn("CreatedOn");
            esq.AddColumn("StSequenceNumber");
            var createdbycol = esq.AddColumn("CreatedBy.Name");

            // var c = esq.AddColumn("StContact");
            // var contact = esq.AddColumn("StContact.Id");

            if (contactId != null) { 
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContact", contactId);
            }
            else if (accountId != null)
            {
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StAccount", accountId);
            }
            else
                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esq.Filters.Add(esqFirstFilter);

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
                        step = (string)entity.GetColumnValue("StStep"),
                        technicalStep = (string)entity.GetColumnValue("StTechnicalStep"),
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

        public void UpdateCustomerAddress(string city,string firstName,string lastName,string addressSeq,string customerId, string contactId, string accountId, string requestId)
        {

            SOMRequestInput<CustomerAddressInput> somRequestInput = new SOMRequestInput<CustomerAddressInput>
            {
                InputArguments = new CustomerAddressInput
                {
                    City = "aaaa",
                    FirstName = firstName,
                    LastName = lastName,
                    AddressSeq = addressSeq,
                    CommonInputArgument = new CommonInputArgument()
                    {
                        ContactId = contactId,
                        CustomerId = customerId,
                        RequestId = requestId
                    }
                }
            };

            using (var client = new SOMClient())
            {
                client.Post<SOMRequestInput<CustomerAddressInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomerAddress/StartProcess", somRequestInput);
            }

        }

        public void UpdateCustomerPaymentProfile(string paymentMethodId, string bankCode, string customerId, string accountNumber, string contactId, string accountId, string requestId)
        {

            SOMRequestInput<CustomerPaymentProfileInput> somRequestInput = new SOMRequestInput<CustomerPaymentProfileInput>
            {
                InputArguments = new CustomerPaymentProfileInput
                {
                    AccountNumber = accountNumber,
                    PaymentMethodId = paymentMethodId,
                    BankCode = bankCode,
                    CommonInputArgument = new CommonInputArgument()
                    {
                        ContactId = contactId,
                        CustomerId = customerId,
                        RequestId = requestId
                    }
                }

            };

            using (var client = new SOMClient())
            {
                client.Post<SOMRequestInput<CustomerPaymentProfileInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomerPaymentProfile/StartProcess", somRequestInput);
            }

        }

        public void UpdateCustomerCategory(string customerCategoryId, string customerId, string requestId)
        {
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
                client.Post<SOMRequestInput<CustomerCategoryInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateCustomer/StartProcess", somRequestInput);
            }

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
            string linePathId,serviceResourceId="";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StNumberToReserve");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StLineType");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
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
                string pathId = entities[0].GetColumnValue("StLinePathID").ToString();
                var lineType = entities[0].GetColumnValue("StLineType");
                var city = entities[0].GetColumnValue("StCityName");
                
                CRMCustomerInfo info = GetCRMCustomerInfo(contactId.ToString(), null);

                if(coreServices != "\"\"") listOfCoreServices= JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
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
                try { sponsor = entities[0].GetTypedColumnValue<Guid>(sponsorcol.Name); }
                catch { }
                bool isForeigner = IsContactForeigner(new Guid(contactId.ToString()));
                List<DepositDocument> depositServices = new List<DepositDocument>();
                if(isForeigner && ( sponsor == Guid.Empty))
                {
                    depositServices = new CatalogManager().GetForeignerDeposits(optionalServices);
                }
                //call api
                SOMRequestInput<TelephonyContractOnHoldInput> somRequestInput = new SOMRequestInput<TelephonyContractOnHoldInput>
                {
                    InputArguments = new TelephonyContractOnHoldInput
                    {
                        LinePathId = linePathId,//"11112222",
                        PhoneNumber = phoneNumber.ToString(),
                        SubType = lineType.ToString(),
                        ServiceResource = serviceResourceId,
                        City = city.ToString(),
                        CSO = info.csoId,
                        RatePlanId = ratePlanId,//ratePlanId.ToString(),
                        ContractServices = contractServices,
                        DepositServices = depositServices,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContactId = contactId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = info.CustomerId //"CusId00026"
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<TelephonyContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateTelephonyContract/StartProcess", somRequestInput);
                }


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
            var nationalitycol = esq.AddColumn("StNationality.Id");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isForeigner = entities[0].GetTypedColumnValue<Guid>(nationalitycol.Name) == Guid.Parse("BC3E1014-D6BA-42E1-BB2D-CF9F40E6B241") ? false : true;
            }
            return isForeigner;
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
