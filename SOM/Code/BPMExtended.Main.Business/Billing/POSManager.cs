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

        public SubmitToPOSResponse SubmitToPOS(string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SubmitToPOSResponse item=null;
            bool depositFlag;
            string contractId,contactId, fees,deposits;
            Contact contact;
            List<SaleService> services = new List<SaleService>();


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager,  new CRMCustomerManager().GetEntityNameByRequestId(requestId));
            var stContact = esq.AddColumn("StContact.Id");
            esq.AddColumn("StContractId");
            esq.AddColumn("StDepositFlag");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StOperationAddedDeposites");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contactId = entities[0].GetTypedColumnValue<string>(stContact.Name).ToString();
                contractId = entities[0].GetColumnValue("StContractId").ToString();
                depositFlag = (bool)entities[0].GetColumnValue("StDepositFlag");
                fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                deposits = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();
                contact = new CommonManager().GetContact(contactId);

                if (fees != "" && fees != null)
                {
                    services = JsonConvert.DeserializeObject<List<SaleService>>(fees);
                }

                var somRequestInput = new SubmitToPOSInput()
                {
                    ContractId = contractId,
                    CustomerCode = contact.CustomerCode,
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
    }
}
