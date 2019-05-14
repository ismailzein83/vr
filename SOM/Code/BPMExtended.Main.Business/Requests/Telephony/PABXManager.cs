using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
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
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", items.Select(p=>p.Id).ToArray());
            esqFirstFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.NotEqual, "StRequestType", telephonyOperationsIds.ToArray());

            esq.Filters.Add(esqFirstFilter);
            esq.Filters.Add(esqFirstFilter2);

            entities = esq.GetEntityCollection(BPM_UserConnection);

              var secondaryContracts = from r in entities.AsEnumerable()
                                       group r by new
                                        {
                                           groupByContractID = r.GetColumnValue("StContractID").ToString()
                                        }
                                        into g
                                        select new
                                          {
                                            contractId = g.Key.groupByContractID
                                          };


            foreach (var secondaryContractId in secondaryContracts)
            {

                filteredItems.Add(new TelephonyContractInfo()
                {
                    Id= secondaryContractId.contractId,
                    PhoneNumber = items.Where(e=>e.Id == secondaryContractId.contractId).Select(e=>e.PhoneNumber).First()
                });

            }

            return filteredItems;
        }


        public PabxContractDetail GetPabxContract(string contractId, string phoneNumber)
        {

            var item = new PabxContractDetail();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<PabxContractDetail>(String.Format("api/SOM.ST/Billing/GetPabxContract?ContractId={0}&PhoneNumber={1}", contractId,phoneNumber));
            }

            return item;
        }


        public ServiceParameter GetPabxServiceParameterValues(string contractId)
        {

            var item = new ServiceParameter();
            using (SOMClient client = new SOMClient())
            {
                item = client.Get<ServiceParameter>(String.Format("api/SOM.ST/Billing/GetPabxServiceParameterValues?ServiceId={0}", contractId));
            }

            return item;

        }


        #endregion


    }
}
