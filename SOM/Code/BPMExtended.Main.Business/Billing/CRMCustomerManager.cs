using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.DB;
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
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            CRMCustomerInfo info;

            if (contactId != null)
            {
                info = GetCRMCustomerInfo(contactId, accountId);
            }
            else
            {
                //account
                info = GetCRMCustomerInfo(contactId, accountId);
            }

            // check categories catalog
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StSkip");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryId", info.CustomerCategoryID);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
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

        public bool IsCustomerCategoryNormal(string customerCategoryId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            bool isNormal = false;

            //Call Categories catalog and check the 'IsNormal' field if true => no need for attachments (optional), if false => attachment is required 
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StIsNormal");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryId", customerCategoryId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isNormal = (bool)entities[0].GetColumnValue("StIsNormal");

            }


            return isNormal;
        }

        public OutputResult NeedsAttachment(string contactId , string accountId , string customerCategoryId)
        {
            bool isNormal;
            string msg = "";
            ResultStatus status = ResultStatus.Success;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

                isNormal = IsCustomerCategoryNormal(customerCategoryId);

                if(isNormal)
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

        public void ChangeADSLLineTerminationRequestHeaderStatusToCompleted(Guid requestId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StRequestHeader").Set("StStatusId", Column.Parameter("8057E9A4-24DE-484D-B202-0D189F5B7758"))
                .Where("StRequestId").IsEqual(Column.Parameter(requestId));
            update.Execute();

            //TODO : Get taxes
            //TODO: If the contract has active VPN service, CRM should add another OCC/fees
        }

        public void ChangeLeasedLineSpeedRequestHeaderStatusToCompleted(Guid requestId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StRequestHeader").Set("StStatusId", Column.Parameter("8057E9A4-24DE-484D-B202-0D189F5B7758"))
                .Where("StRequestId").IsEqual(Column.Parameter(requestId));
            update.Execute();

            //TODO:OM should deactivate the unavailable services in the new rateplan(if any based on the catalog).
            //TODO:OM should add the new core services in the new ratplan, if they were not activated before(this case is not exist based on the posted catalog).

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

                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Name" , Description = name.ToString()});
                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Document Id", Description = documentId.ToString()});
                    customerInfo.Add(new CRMCustomerInfoLabelAutoGenerated() { Label = "Customer Category", Description = new RatePlanManager().GetCustomerCategoryById(customerCategoryId.ToString()).Name});

                }

            }
            else
            {
                //return ;
            }


            return customerInfo;

        }

        public CRMCustomerInfo GetCRMCustomerInfo(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            CRMCustomerInfo customerInfo = null;

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

                    customerInfo = new CRMCustomerInfo()
                    {
                        CustomerId = customerId.ToString(),
                        CustomerName = name.ToString(),
                        DocumentID = documentId.ToString(),
                        CustomerCategoryID = customerCategoryId.ToString(),
                        CustomerCategoryName = new RatePlanManager().GetCustomerCategoryById(customerCategoryId.ToString()).Name


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


                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "SysUserInRole");
                esq.AddColumn("SysUser");
                var cashierColumn =  esq.AddColumn("SysUser.Contact.StCSO.StCashier");

                //var csoidcol = esq.AddColumn("StCSO.Id");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysUser", sysUserId);
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
    }
}
