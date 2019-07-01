﻿using System;
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

        public SubmitToPOSResponse SubmitToPOS(string requestId , string fees)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SubmitToPOSResponse item=null;
            bool depositFlag;
            string contractId=null,contactId;
            Contact contact;
            EntityCollection entities;
            List<SaleService> services = new List<SaleService>();


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StContractID");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
            esq.Filters.Add(esqFirstFilter);

             entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contractId = entities[0].GetColumnValue("StContractID").ToString();
            }


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager,  new CRMCustomerManager().GetEntityNameByRequestId(requestId));
            var stContact = esq.AddColumn("StContact.Id");
            esq.AddColumn("StDepositFlag");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

             entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contactId = entities[0].GetTypedColumnValue<string>(stContact.Name).ToString();             
                depositFlag = (bool)entities[0].GetColumnValue("StDepositFlag");
                contact = new CommonManager().GetContact(contactId);

                if (fees != "" && fees != null && fees != "[]")
                {
                    services = JsonConvert.DeserializeObject<List<SaleService>>(fees);
                }

                var somRequestInput = new SubmitToPOSInput()
                {
                    ContractId = contractId,
                    CustomerCode = new CRMCustomerManager().GetCustomerInfo(contact.CustomerId).CustomerCode,
                    DepositFlag = depositFlag,
                    Services = services.Where(s=>s.UpFront==true).ToList()//filter the fees by upfront (just send the fees that have the UpFront == true)
                };

                //call api
                using (var client = new SOMClient())
                {
                    item = client.Post<SubmitToPOSInput,SubmitToPOSResponse>("api/SOM.ST/Billing/SubmitToPOS", somRequestInput);
                }
            }

            return item;
                    
        }

        public bool ValidatePosPayment(string caseId, string requestId)
        {
            //TODO: check if user has paid
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            Contact contact;
            bool item=false;
            string contactId;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, new CRMCustomerManager().GetEntityNameByRequestId(requestId));
            var stContact = esq.AddColumn("StContact.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contactId = entities[0].GetTypedColumnValue<string>(stContact.Name).ToString();
                contact = new CommonManager().GetContact(contactId);

                using (SOMClient client = new SOMClient())
                {
                    item = client.Get<bool>(String.Format("api/SOM.ST/Billing/ValidatePayment?CaseId={0}&CustomerCode={1}", caseId, new CRMCustomerManager().GetCustomerInfo(contact.CustomerId).CustomerCode));
                }
            }          
            return item;
        }
    }
}
