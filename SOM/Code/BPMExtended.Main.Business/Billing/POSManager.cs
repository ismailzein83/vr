using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class POSManager
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

        public List<SubmitToPOSResponse> SubmitToPOS(string requestId , string fees)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<SubmitToPOSResponse> items=null;
            bool depositFlag;
            string contractId=null,contactId,accountId;
            Contact contact = new Contact();
            Account account = new Account();
            EntityCollection entities;
            List<SaleService> services = new List<SaleService>();

            string entityName = new CRMCustomerManager().GetEntityNameByRequestId(requestId);
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            esq.AddColumn("StContractID");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

             entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contractId = entities[0].GetColumnValue("StContractID").ToString();
            }


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager,  new CRMCustomerManager().GetEntityNameByRequestId(requestId));
            var stContact = esq.AddColumn("StContact.Id");
            var stAccount = esq.AddColumn("StAccount.Id");
            esq.AddColumn("StDepositFlag");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

             entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contactId = entities[0].GetTypedColumnValue<string>(stContact.Name).ToString();
                accountId = entities[0].GetTypedColumnValue<string>(stAccount.Name).ToString();
                depositFlag = (bool)entities[0].GetColumnValue("StDepositFlag");

                if(contactId != null && contactId != "")
                    contact = new CommonManager().GetContact(contactId);
                else
                    account = new CommonManager().GetAccount(accountId);


                if (fees != "" && fees != null && fees != "[]")
                {
                    services = JsonConvert.DeserializeObject<List<SaleService>>(fees);
                }


                var somRequestInput = new SubmitToPOSInput()
                {
                    ContractId = contractId,
                    CustomerCode = contactId!=null && contactId != "" ?  new CRMCustomerManager().GetCustomerInfo(contact.CustomerId).CustomerCode : new CRMCustomerManager().GetCustomerInfo(account.CustomerId).CustomerCode,
                    DepositFlag = depositFlag,
                    Services = services.Where(s=>s.UpFront==true).ToList()//filter the fees by upfront (just send the fees that have the UpFront == true)
                };

                //call api
                using (var client = new SOMClient())
                {
                    items = client.Post<SubmitToPOSInput, List<SubmitToPOSResponse>>("api/SOM.ST/Billing/SubmitToPOS", somRequestInput);
                }
            }

            return items;
                    
        }

        public ValidatePaymentState ValidatePosPayment(string caseId, string requestId , string depositId)
        {
            //TODO: check if user has paid
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            Contact contact = new Contact();
            Account account = new Account();
            ValidatePaymentState item ;
            string contactId, accountId,customerId;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, new CRMCustomerManager().GetEntityNameByRequestId(requestId));
            var stContact = esq.AddColumn("StContact.Id");
            var stAccount = esq.AddColumn("StAccount.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contactId = entities[0].GetTypedColumnValue<string>(stContact.Name).ToString();
                accountId = entities[0].GetTypedColumnValue<string>(stAccount.Name).ToString();

                if (contactId != null && contactId != "")
                    customerId = new CommonManager().GetContact(contactId).CustomerId;
                else
                    customerId = new CommonManager().GetAccount(accountId).CustomerId;

                using (SOMClient client = new SOMClient())
                {
                    item = client.Get<ValidatePaymentState>(String.Format("api/SOM.ST/Billing/ValidatePayment?CaseId={0}&CustomerCode={1}&CustomerId={1}&DepositId={1}", caseId, new CRMCustomerManager().GetCustomerInfo(customerId).CustomerCode, customerId, depositId));
                }
                return item;
            }     
            return ValidatePaymentState.Undefined;
        }
    }
}
