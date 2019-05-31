using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CommonManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
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

        public int GetNumberOfAttachments(string schemaName , string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, schemaName+"File");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, schemaName+".Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities != null)
            {
                return entities.Count;
            }

            return 0;
        }

        public Contact GetContact(string contactId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            Contact contact = null;

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

                contact = new Contact()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString(),
                    DocumentID = documentId.ToString(),
                    CustomerCategoryID = customerCategoryId.ToString(),
                    CustomerCategoryName = customerCategoryName.ToString(),
                    CSOId = csoId.ToString()


                };
            }
            return contact;
        }

        public Account GetAccount(string accountId)
        {
            return null;
        }

    }
}
