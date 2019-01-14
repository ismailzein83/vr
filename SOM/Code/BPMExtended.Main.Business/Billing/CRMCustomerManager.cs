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
    public class CRMCustomerManager
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
            string msg = "";
            ResultStatus status;
            CRMCustomerInfo info;

            //get customer info (CRM)
            if (contactId != null)
            {
                info = GetCRMCustomerInfo(contactId, null);
            }
            else
            {
                info = GetCRMCustomerInfo(null, accountId);
            }

            // check categories catalog
            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StSkip");

            var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryId", info.CustomerCategoryId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isSkip = (bool)entities[0].GetColumnValue("StSkip");
            }

            if (isSkip)
            {
                msg = "Operation is allowed";
                status = ResultStatus.Success;
            }
            else
            {
                //check customer balance
                if (new BillingManager().GetCustomerBalance(info.CustomerId).Balance > 0)
                {
                    msg = "You must pay all invoices before proceeding this operation";
                    status = ResultStatus.Error;
                }
                else
                {
                    msg = "Operation is allowed";
                    status = ResultStatus.Success;
                }

            }

            return new OutputResult()
            {
                messages = new List<string>() { msg },
                status = status
            };

        }


        public bool NeedsAttachment(string customerCategoryId)
        {
            bool isNormal = false;

            //Call Categories catalog and check the 'IsNormal' field if true => no need for attachments (optional), if false => attachment is required 
            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StIsNormal");

            var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryId", customerCategoryId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isNormal = (bool)entities[0].GetColumnValue("StIsNormal");
            }

            return isNormal;
        }

        public CRMCustomerInfo GetCRMCustomerInfo(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            //Get infos from contact table in CRM database 

            if (contactId != null)
            {
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("Name");
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryID");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                    return new CRMCustomerInfo()
                    {
                        CustomerName = name.ToString(),
                        CustomerId = customerId.ToString(),
                        DocumentId = documentId.ToString(),
                        CustomerCategoryId = customerCategoryId.ToString(),
                        CustomerCategoryName = new RatePlanManager().GetCustomerCategoryById(customerCategoryId.ToString()).Name
                    };

                }

            }
            else
            {
                //return ;
            }


            return new CRMCustomerInfo();

        }


        public PaymentMethod CheckCSO(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            if (contactId != null)
            {
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("StCSO");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var csoId = entities[0].GetColumnValue("StCSO");

                    esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCSO");
                    esq.AddColumn("StCashier");
                    esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                    esq.Filters.Add(esqFirstFilter);
                    var entities2 = esq.GetEntityCollection(BPM_UserConnection);
                    if (entities2.Count > 0)
                    {
                        bool isCashier = (bool)entities[0].GetColumnValue("StCashier");

                        if (isCashier)
                            return new PaymentMethod()
                            {
                                Id = "E78BC2E8-119B-475B-AFB1-962B11842597",
                                name = "Cash"
                            };

                        else
                            return new PaymentMethod()
                            {
                                Id = "E9F030F4-BF08-4E99-96C2-86C92134FC0F",
                                name = "Invoice"
                            };
                    }
                }

            }
            else
            {
                //account
            }

            return null;
        }
    }
}
