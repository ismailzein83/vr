using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class PABXManager
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

        #region Public

        public List<TelephonyContractInfo> GetValidPabxPhoneNumbers(string contractId , string customerId)
        {
            EntitySchemaQuery esq;
            EntityCollection entities;
            IEntitySchemaQueryFilterItem esqFirstFilter, esqFirstFilter2;
            List<TelephonyContractInfo> items = new List<TelephonyContractInfo>();
            List<TelephonyContractInfo> filteredItems = new List<TelephonyContractInfo>();
            var telephonyOperationsIds = new List<string>();
            
            //Get pabx Service Id from general settings catalog
            string pabxServiceId = new CatalogManager().GetPABXServiceId();

            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<TelephonyContractInfo>>(String.Format("api/SOM.ST/Billing/GetValidPabxPhoneNumbers?ContractId={0}&PABXServiceId={1}&CustomerId={2}", contractId, pabxServiceId,customerId));
            }

            //Get telephony operations ids Id from general settings catalog
            telephonyOperationsIds = new CatalogManager().GetTelephonyOperations();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StRequestType");
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", items.Select(p=>p.Id).ToArray());
            esqFirstFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", telephonyOperationsIds.ToArray());

            esq.Filters.Add(esqFirstFilter);
            esq.Filters.Add(esqFirstFilter2);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                string secondaryContractId = (string)entity.GetColumnValue("StContractID");

                filteredItems.Add(new TelephonyContractInfo()
                {
                    Id= secondaryContractId,
                    PhoneNumber = items.Where(e=>e.Id == secondaryContractId).Select(e=>e.PhoneNumber).ToString()
                });

            }

            return filteredItems;
        }



        #endregion


    }
}
