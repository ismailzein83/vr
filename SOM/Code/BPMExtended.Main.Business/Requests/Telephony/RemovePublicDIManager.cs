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
    public class RemovePublicDIManager
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
        public void PostToOM(Guid requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRemovePublicDI");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                List<ServiceData> servicesList = new List<ServiceData>();

                CatalogManager cmanager = new CatalogManager();
                servicesList = cmanager.GetAddPublicDIServices();

                SOMRequestInput<AddPublicDIInput> somRequestInput = new SOMRequestInput<AddPublicDIInput>
                {
                    InputArguments = new AddPublicDIInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            CustomerId = customerId.ToString(),
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        ServicesList = servicesList
                    }
                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<AddPublicDIInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/RemoveContractFromDirectoryInquiry/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public string GetProfessionalDIServiceContracted(string contractId)
        {
            string serviceId;
            using (SOMClient client = new SOMClient())
            {
                serviceId = client.Get<string>(String.Format("api/SOM.ST/Billing/GetPrivateDIServiceContracted?contractId={0}", contractId));
            }

            return string.IsNullOrEmpty(serviceId)? "" : serviceId;
        }
        #endregion
    }
}
