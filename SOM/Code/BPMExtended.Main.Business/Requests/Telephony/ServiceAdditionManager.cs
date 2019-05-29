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
    public class ServiceAdditionManager
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

        public List<ContractAvailableServiceOutput> GetContractAvailableServices(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<ContractAvailableServiceOutput> items = new List<ContractAvailableServiceOutput>();
            List<ContractAvailableServiceOutput> filteredServices = new List<ContractAvailableServiceOutput>();
            List<string> packagesIds = new List<string>();


            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            packagesIds.Add(packages.Core);
            packagesIds.Add(packages.Telephony);


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceAdditionRequest");
            esq.AddColumn("StPathId");
            esq.AddColumn("StContractId");
            esq.AddColumn("StRatePlanId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StPathId");
                var contractId = entities[0].GetColumnValue("StContractId");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");

                var somRequestInput = new ContractAvailableServicesInput()
                {
                    ContractId = contractId.ToString(),
                    RatePlanId = ratePlanId.ToString(),//"TM005",
                    LinePathId = pathId.ToString(),//"184",
                    ExcludedPackages = packagesIds
                };


                //call api
                using (var client = new SOMClient())
                {
                    items = client.Post<ContractAvailableServicesInput, List<ContractAvailableServiceOutput>>("api/SOM.ST/Billing/GetContractAvailableServices", somRequestInput);
                }

            }

            //Get special services from service definition catalog 
            List<string> specialServicesIds = new CatalogManager().GetSpecialServicesIds();

            //filter the ContractAvailableServices (ContractAvailableServices - special services)
            filteredServices = (from item in items
                                where !specialServicesIds.Contains(item.Id)
                                select item).ToList();



            return filteredServices;

        }

        public void PostAddAdditionalServicesRequestToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceAdditionRequest");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StPathId");
            esq.AddColumn("StServices");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StOperationAddedDeposites");
            esq.AddColumn("StOperationAddedServices");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StPathId");
                //string contractAdditionalServices = entities[0].GetColumnValue("StServices").ToString();
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string deposits = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();
                string VASServices = entities[0].GetColumnValue("StOperationAddedServices").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ServiceAdditionRequestInput> somRequestInput = new SOMRequestInput<ServiceAdditionRequestInput>
                {

                    InputArguments = new ServiceAdditionRequestInput
                    {
                        LinePathId = pathId.ToString(),
                        ContractAdditionalServices = JsonConvert.DeserializeObject<List<VASService>>(VASServices),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ServiceAdditionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_AddAdditionalServices/StartProcess", somRequestInput);
                }

            }

        }

        #endregion

    }
}
