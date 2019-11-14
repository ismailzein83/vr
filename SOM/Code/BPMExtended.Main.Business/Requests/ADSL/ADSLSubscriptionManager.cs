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
    public class ADSLSubscriptionManager
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

        public bool HasTelephonyContractWithNoADSL(string customerId)
        {
            bool result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Billing/HasTelephonyContractWithNoADSL?CustomerId={0}", customerId));
            }
            return result;
        }

        public List<TelephonyContractInfo> GetTelephonyContractsWithNoADSL(string customerId)
        {
            List<TelephonyContractInfo> result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<List<TelephonyContractInfo>>(String.Format("api/SOM.ST/Billing/GetTelephonyContractsWithNoADSL?CustomerId={0}", customerId));
            }
            return result;
        }

        public bool CheckADSLRequestInWaitingList()
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqSecondFilter, esqThirdFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StRequestId");

            esqSecondFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus.Id", "8BC51DE4-23FE-4CF9-93A7-C7D9CC35C5AC");
            esqThirdFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", "5");
            esq.Filters.Add(esqSecondFilter);
            esq.Filters.Add(esqThirdFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                //Guid requestId = entities[0].GetTypedColumnValue<Guid>("StRequestId");
                return true;
            }
            return false;
        }

        public SOMRequestOutput CreateADSLContractOnHold(Guid requestId, string coreServices, string optionalServices, string ratePlanId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output = new SOMRequestOutput();
            List<ContractServiceInfo> contractServices = new List<ContractServiceInfo>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            List<DepositDocument> depositServices = new List<DepositDocument>();
            string linePathId, serviceResourceId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");
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
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                string pathId = entities[0].GetColumnValue("StLinePathID").ToString();
                var city = entities[0].GetColumnValue("StCityName");
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var town = entities[0].GetColumnValue("StTownName");
                var contractId = entities[0].GetColumnValue("StContractID");
                var username = entities[0].GetColumnValue("StUserName");
                var locationType= entities[0].GetColumnValue("StLocationName");

                CRMCustomerInfo info = contactId != null ? new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null) : new CRMCustomerManager().GetCRMCustomerInfo(null, accountId.ToString());

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                /*  Dictionary<string, string> servicesDiscount = new CatalogManager().GetServicesDiscount();


                  depositServices = (from item in listOfOptionalServices
                                     where item.HasDeposit
                                     select new DepositDocument() { Id = item.Id }).ToList();


                  listOfOptionalServices = (from item in listOfOptionalServices
                                            where item.HasDiscount
                                            select new ServiceDetail() { Id = servicesDiscount[item.Id] }).ToList();*/



                var items = listOfCoreServices.Concat(listOfOptionalServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in items)
                {
                    var contractServiceItem = ServiceToContractServiceMapper(item);
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
                SOMRequestInput<ADSLContractOnHoldInput> somRequestInput = new SOMRequestInput<ADSLContractOnHoldInput>
                {
                    InputArguments = new ADSLContractOnHoldInput
                    {
                        LinePathId = linePathId,//"11112222",
                        ServiceResource = serviceResourceId,
                        UserName= username.ToString(),
                        City = city.ToString(),
                        TelephonyContractId = contractId.ToString(),
                        Building = buildingNumber.ToString(),
                        Floor = floor.ToString(),
                        Town = town.ToString(),
                        StateProvince = province.ToString(),
                        Street = street.ToString(),
                        Region = area.ToString(),
                        CSO = info.csoBSCSId,
                        SubType =  "ADSL",
                        CountryId= "206",
                        RatePlanId = ratePlanId,
                        LocationType= locationType.ToString(),
                        DepositServices = depositServices,
                        Services = contractServices,
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
                    output = client.Post<SOMRequestInput<ADSLContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateADSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }

            return output;
        }


        public void SubmitActivateADSLContract(Guid requestId)
        {

            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StUserName");
            esq.AddColumn("StPassword");
            esq.AddColumn("StRatePlanId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                string coreServices = entities[0].GetColumnValue("StCoreServices").ToString();
                string optionalServices = entities[0].GetColumnValue("StServices").ToString();
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var userName = entities[0].GetColumnValue("StUserName");
                var password = entities[0].GetColumnValue("StPassword");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);


                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);
                }


                SOMRequestInput<ADSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<ADSLSubscriptionRequestInput>
                {

                    InputArguments = new ADSLSubscriptionRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = customerId.ToString()
                        },
                        LinePathId = pathId.ToString(),
                        ContractServices = contractServices,
                        IsVPN = new CatalogManager().GetDivisionByRatePlanId(ratePlanId.ToString()) == "VPN" ? true : false,
                        Username = userName.ToString(),
                        Password= password.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateADSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }

        public void FinalizeActivateADSLContract(Guid requestId)
        {

            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StUserName");
            esq.AddColumn("StPassword");
            esq.AddColumn("StRatePlanId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");
                string coreServices = entities[0].GetColumnValue("StCoreServices").ToString();
                string optionalServices = entities[0].GetColumnValue("StServices").ToString();
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StLinePathID");
                var userName = entities[0].GetColumnValue("StUserName");
                var password = entities[0].GetColumnValue("StPassword");
                var ratePlanId = entities[0].GetColumnValue("StRatePlanId");

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);


                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);
                }


                SOMRequestInput<ADSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<ADSLSubscriptionRequestInput>
                {

                    InputArguments = new ADSLSubscriptionRequestInput
                    {
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        LinePathId = pathId.ToString(),
                        ContractServices = contractServices,
                        IsVPN = new CatalogManager().GetDivisionByRatePlanId(ratePlanId.ToString()) == "VPN" ? true : false,
                        Username = userName.ToString(),
                        Password = password.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeActivateADSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void SubmitActivateServicesADSLContract(Guid requestId)
        {

            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                string coreServices = entities[0].GetColumnValue("StCoreServices").ToString();
                string optionalServices = entities[0].GetColumnValue("StServices").ToString();
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var pathId = entities[0].GetColumnValue("StLinePathID");

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);
                if (optionalServices != "\"\"") listOfOptionalServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(optionalServices);

                var items = listOfCoreServices.Concat(listOfOptionalServices);


                foreach (var item in items)
                {
                    var contractServiceItem = ServiceDetailToContractServiceMapper(item);
                    contractServices.Add(contractServiceItem);
                }


                SOMRequestInput<ADSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<ADSLSubscriptionRequestInput>
                {

                    InputArguments = new ADSLSubscriptionRequestInput
                    {
                        ContractId = contractId.ToString(),
                        RequestId = requestId.ToString(),
                        LinePathId = pathId.ToString(),
                        ContractServices = contractServices
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/SubmitActivateServicesXDSLContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }
        }

        public void FinalizeActivateServicesADSLContract(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<ADSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<ADSLSubscriptionRequestInput>
                {
                    
                    InputArguments = new ADSLSubscriptionRequestInput
                    {
                        RequestId = requestId.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/FinalizeActivateServicesXDSLContract/StartProcess", somRequestInput);
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
        public ContractServiceInfo ServiceToContractServiceMapper(ServiceDetail item)
        {
            return new ContractServiceInfo
            {
                Id = item.Id,
                PackageId = item.PackageId
            };
        }

        #endregion
    }
}
