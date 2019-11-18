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
            esq.AddColumn("StContractID");
            esq.AddColumn("StRatePlanId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var pathId = entities[0].GetColumnValue("StPathId");
                var contractId = entities[0].GetColumnValue("StContractID");
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
            esq.AddColumn("StContractID");
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
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StPathId");
                //string contractAdditionalServices = entities[0].GetColumnValue("StServices").ToString();
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string deposits = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();
                string VASServices = entities[0].GetColumnValue("StOperationAddedServices").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                List<ContractServiceInfo> contractAddedServices = new List<ContractServiceInfo>();

                List<VASService> vasServices = JsonConvert.DeserializeObject<List<VASService>>(VASServices);
                if (vasServices != null)
                {
                    foreach (var vService in vasServices)
                    {
                        contractAddedServices.Add(ServiceDetailToContractServiceMapper(vService));
                    }
                }
                SOMRequestInput<ServiceAdditionRequestInput> somRequestInput = new SOMRequestInput<ServiceAdditionRequestInput>
                {

                    InputArguments = new ServiceAdditionRequestInput
                    {
                        LinePathId = pathId.ToString(),
                        Services = contractAddedServices,
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

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void SubmitAdditionalServices(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceAdditionRequest");
            esq.AddColumn("StContractID");
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
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StPathId");
                //string contractAdditionalServices = entities[0].GetColumnValue("StServices").ToString();
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string deposits = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();
                string VASServices = entities[0].GetColumnValue("StOperationAddedServices").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                List<ContractServiceInfo> contractAddedServices = new List<ContractServiceInfo>();

                List<VASService> vasServices = JsonConvert.DeserializeObject<List<VASService>>(VASServices);
                if (vasServices != null)
                {
                    foreach(var vService in vasServices)
                    {
                        contractAddedServices.Add(ServiceDetailToContractServiceMapper(vService));
                    }
                }
                SOMRequestInput<ServiceAdditionRequestInput> somRequestInput = new SOMRequestInput<ServiceAdditionRequestInput>
                {

                    InputArguments = new ServiceAdditionRequestInput
                    {
                        LinePathId = pathId.ToString(),
                        Services = contractAddedServices,
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
                    output = client.Post<SOMRequestInput<ServiceAdditionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitAddAdditionalServices/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FinalizeAdditionalServices(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceAdditionRequest");
            esq.AddColumn("StContractID");
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
                var contractId = entities[0].GetColumnValue("StContractID");
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
                    output = client.Post<SOMRequestInput<ServiceAdditionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeAddAdditionalServices/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        public ContractServiceInfo ServiceDetailToContractServiceMapper(VASService item)
        {
            ContractServiceInfo contractServiceInfo = new ContractServiceInfo
            {
                Id = item.Id,
                PackageId = item.PackageId,
                Parameters = new List<ContractServiceParameter>()
            };

            if (item.Parameters != null)
            {
                foreach (var seviceParameter in item.Parameters)
                {
                    ContractServiceParameter contractServiceParameter = new ContractServiceParameter();
                    contractServiceParameter.Description = seviceParameter.ParameterName;
                    contractServiceParameter.Id = seviceParameter.Id;
                    contractServiceParameter.ParameterNumber = seviceParameter.ParameterNumber;
                    contractServiceParameter.Type = seviceParameter.Type;
                    contractServiceParameter.Values = new List<ContractServiceParameterValue> {
                        new ContractServiceParameterValue {
                        Description = seviceParameter.ParameterDisplayValue,
                        Value=seviceParameter.ParameterValue,
                        SequenceNumber=seviceParameter.SequenceNumber
                    }
                    };
                    contractServiceInfo.Parameters.Add(contractServiceParameter);
                }
            }
            return contractServiceInfo;
        }

        #endregion

    }
}
