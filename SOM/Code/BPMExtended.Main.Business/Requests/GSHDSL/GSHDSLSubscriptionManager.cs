using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class GSHDSLSubscriptionManager
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

        public string HasPendingRequestInWaitingList(string switchId , string division)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("Id");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StSwitchDeviceId", switchId);
            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StGSHDSLDivision", division);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStep.Id", "ED5E126A-3336-47E6-9138-2788FF96B78A");

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var id = entities[0].GetColumnValue("Id");
                return id.ToString();
            }
            return null;
        }

        public SOMRequestOutput CreateGSHDSLContract(Guid requestId, string coreServices, string optionalServices, string ratePlanId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output = new SOMRequestOutput();
            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            List<DepositDocument> depositServices = new List<DepositDocument>();
            string linePathId, serviceResourceId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathId");
            esq.AddColumn("StContractID");
            esq.AddColumn("StUserName");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                string pathId = entities[0].GetColumnValue("StLinePathId").ToString();
                var city = entities[0].GetColumnValue("StCityName");
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var contractId = entities[0].GetColumnValue("StContractID");
                var username = entities[0].GetColumnValue("StUserName");

                CRMCustomerInfo info = contactId != null ? new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null) : new CRMCustomerManager().GetCRMCustomerInfo(null, accountId.ToString());

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);


                var items = listOfCoreServices.Concat(listOfOptionalServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);
                }


                if (pathId.EndsWith(".0"))
                {
                    linePathId = pathId.Substring(0, pathId.Length - 2);
                }
                else
                {
                    linePathId = pathId;
                }

                //call api
                SOMRequestInput<GSHDSLContractOnHoldInput> somRequestInput = new SOMRequestInput<GSHDSLContractOnHoldInput>
                {
                    InputArguments = new GSHDSLContractOnHoldInput
                    {
                        LinePathId = linePathId,//"11112222",
                        ServiceResource = serviceResourceId,
                        UserName = username.ToString(),
                        City = city.ToString(),
                        Building = buildingNumber.ToString(),
                        Floor = floor.ToString(),
                        Town = town.ToString(),
                        StateProvince = province.ToString(),
                        Street = street.ToString(),
                        Region = area.ToString(),
                        CSO = info.csoId,
                        SubType = "XDSL",
                        CountryId = "206",
                        RatePlanId = ratePlanId,
                        ContractServices = contractServices,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContactId = contactId != null ? contactId.ToString() : null,
                            AccountId = accountId != null ? accountId.ToString() : null,
                            RequestId = requestId.ToString(),
                            CustomerId = info.CustomerId
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<GSHDSLContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateGSHDSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }

            return output;
        }


        public void PostGSHDSLRequestToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<GSHDSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<GSHDSLSubscriptionRequestInput>
                {

                    InputArguments = new GSHDSLSubscriptionRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<GSHDSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateGSHDSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void ActivateGSHDSLRequestToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGSHDSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StLinePathId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var linePathId = entities[0].GetColumnValue("StLinePathId");

                SOMRequestInput<GSHDSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<GSHDSLSubscriptionRequestInput>
                {

                    InputArguments = new GSHDSLSubscriptionRequestInput
                    {
                        LinePathId = linePathId.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<GSHDSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        #endregion

        #region Mappers
        public ContractService ServiceDetailToContractServiceMapper(ServiceDetail item)
        {
            return new ContractService
            {
                sncode = item.Id,
                spcode = item.PackageId
            };
        }

        #endregion
    }
}
