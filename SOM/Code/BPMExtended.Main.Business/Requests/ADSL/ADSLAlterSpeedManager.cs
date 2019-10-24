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
    public class ADSLAlterSpeedManager
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

        #region public
        public void PostADSLAlterSpeedToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            string serviceId;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLAlterSpeed");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StRatePlanId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StNewRatePlanId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var oldRatePlanId = entities[0].GetColumnValue("StRatePlanId");
                var pathdId = entities[0].GetColumnValue("StLinePathId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var newRatePlanId = entities[0].GetColumnValue("StNewRatePlanId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);

                //Get ranks
                int oldRank = new CatalogManager().GetRankByRatePlanId(oldRatePlanId.ToString());
                int newRank = new CatalogManager().GetRankByRatePlanId(newRatePlanId.ToString());

                
                if(oldRank < newRank)
                {
                    //get upgrade rank
                    serviceId = new CatalogManager().GetUpgradSpeedServiceId();
                    feesServices.Add(new SaleService() {Id=serviceId,UpFront=false });
                }
                else if (oldRank > newRank)
                {
                    //get downgrade rank
                    serviceId = new CatalogManager().GetDowngradeSpeedServiceId();
                    feesServices.Add(new SaleService() { Id = serviceId, UpFront = false });
                }

                SOMRequestInput<ADSLAlterSpeedRequestInput> somRequestInput = new SOMRequestInput<ADSLAlterSpeedRequestInput>
                {

                    InputArguments = new ADSLAlterSpeedRequestInput
                    {
                        LinePathId = pathdId.ToString(),
                        NewRatePlan = newRatePlanId.ToString(),
                        ServicesAreCoreInNewRP = new ServiceManager().ReadProductChangeServices(contractId.ToString(),newRatePlanId.ToString()).ServicesAreCoreInNewRP,
                        ServicesAreInDifferentPKGInNewRP = new ServiceManager().ReadProductChangeServices(contractId.ToString(), newRatePlanId.ToString()).ServicesAreInDifferentPKGInNewRP,
                        ServicesAreUnavailableInNewRP = new ServiceManager().ReadProductChangeServices(contractId.ToString(), newRatePlanId.ToString()).ServicesAreUnavailableInNewRP,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        PaymentData = new PaymentData()
                        {
                            Fees = feesServices,
                            IsPaid = (bool)isPaid
                        },
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLAlterSpeedRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitADSLAlterSpeed/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }


        public void FinalizeXDSLAlterSpeed(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            string serviceId;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLAlterSpeed");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StRatePlanId");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StNewRatePlanId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var oldRatePlanId = entities[0].GetColumnValue("StRatePlanId");
                var pathdId = entities[0].GetColumnValue("StLinePathId");
                var contractId = entities[0].GetColumnValue("StContractID");
                var newRatePlanId = entities[0].GetColumnValue("StNewRatePlanId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                //fees
                List<SaleService> feesServices = JsonConvert.DeserializeObject<List<SaleService>>(fees);

                //Get ranks
                int oldRank = new CatalogManager().GetRankByRatePlanId(oldRatePlanId.ToString());
                int newRank = new CatalogManager().GetRankByRatePlanId(newRatePlanId.ToString());


                if (oldRank < newRank)
                {
                    //get upgrade rank
                    serviceId = new CatalogManager().GetUpgradSpeedServiceId();
                    feesServices.Add(new SaleService() { Id = serviceId, UpFront = false });
                }
                else if (oldRank > newRank)
                {
                    //get downgrade rank
                    serviceId = new CatalogManager().GetDowngradeSpeedServiceId();
                    feesServices.Add(new SaleService() { Id = serviceId, UpFront = false });
                }

                SOMRequestInput<ADSLAlterSpeedRequestInput> somRequestInput = new SOMRequestInput<ADSLAlterSpeedRequestInput>
                {

                    InputArguments = new ADSLAlterSpeedRequestInput
                    {
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = feesServices,
                            IsPaid = (bool)isPaid
                        },
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLAlterSpeedRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeADSLAlterSpeed/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        #endregion
    }
}
