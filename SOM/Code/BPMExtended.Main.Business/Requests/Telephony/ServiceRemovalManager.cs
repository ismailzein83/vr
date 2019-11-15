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
    public class ServiceRemovalManager
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
        public List<ContractAvailableServiceOutput> GetContractServicesWithExcludedPackages(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<ContractAvailableServiceOutput> items = new List<ContractAvailableServiceOutput>();
            List<string> packagesIds = new List<string>();


            var businessEntityManager = new BusinessEntityManager();
            Packages packages = businessEntityManager.GetServicePackagesEntity();
            packagesIds.Add(packages.Core);
            packagesIds.Add(packages.Telephony);


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceRemoval");
            esq.AddColumn("StContractID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");          
                var somRequestInput = new ServiceRemovalContractServicesInput()
                {
                    ContractId = contractId.ToString(),
                    ExcludedPackages = packagesIds
                };

                //call api
                using (var client = new SOMClient())
                {
                    items = client.Post<ServiceRemovalContractServicesInput, List<ContractAvailableServiceOutput>>("api/SOM.ST/Billing/GetContractServicesWithExcludedPackages", somRequestInput);
                }

            }


            return items;

        }

        public void SubmitServiceRemovalToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceRemoval");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StOperationAddedServices");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StPathId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string VASServices = entities[0].GetColumnValue("StOperationAddedServices").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                var pathId = entities[0].GetColumnValue("StPathId");

                SOMRequestInput<ServiceRemovalSubmitInput> somRequestInput = new SOMRequestInput<ServiceRemovalSubmitInput>
                {

                    InputArguments = new ServiceRemovalSubmitInput
                    {
                        Services = JsonConvert.DeserializeObject<List<VASService>>(VASServices),
                        LinePathId = pathId.ToString(),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ServiceRemovalSubmitInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitRemoveContractServices/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void ActivateServiceRemovalToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StServiceRemoval");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StOperationAddedServices");
            esq.AddColumn("StIsPaid");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string VASServices = entities[0].GetColumnValue("StOperationAddedServices").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ServiceRemovalSubmitInput> somRequestInput = new SOMRequestInput<ServiceRemovalSubmitInput>
                {

                    InputArguments = new ServiceRemovalSubmitInput
                    {
                        //ServicesToRemove = JsonConvert.DeserializeObject<List<VASService>>(VASServices),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees),
                            IsPaid = (bool)isPaid
                        },
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ServiceRemovalSubmitInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeRemoveContractServices/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        #endregion
    }
}
