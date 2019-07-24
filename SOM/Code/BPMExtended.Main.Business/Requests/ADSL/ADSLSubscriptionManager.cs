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

        public string CheckADSLRequestInWaitingListByTelContract(string contractId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter, esqSecondFilter, esqThirdFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StRequestId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esqSecondFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus.Id", "8BC51DE4-23FE-4CF9-93A7-C7D9CC35C5AC");
            esqThirdFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", "5");
            esq.Filters.Add(esqFirstFilter);
            esq.Filters.Add(esqSecondFilter);
            esq.Filters.Add(esqThirdFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                Guid requestId = entities[0].GetTypedColumnValue<Guid>("StRequestId");
                return requestId.ToString();
            }
            return null;
        }

        public SOMRequestOutput CreateADSLContractOnHold(Guid requestId, string coreServices, string optionalServices, string ratePlanId)
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

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineSubscriptionRequest");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                string pathId = entities[0].GetColumnValue("StLinePathID").ToString();
                var city = entities[0].GetColumnValue("StCityName");

                CRMCustomerInfo info = new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null);

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
                SOMRequestInput<TelephonyContractOnHoldInput> somRequestInput = new SOMRequestInput<TelephonyContractOnHoldInput>
                {
                    InputArguments = new TelephonyContractOnHoldInput
                    {
                        LinePathId = linePathId,//"11112222",
                        ServiceResource = serviceResourceId,
                        City = city.ToString(),
                        CSO = info.csoId,
                        RatePlanId = ratePlanId,
                        DepositServices = depositServices,
                        ContractServices = contractServices,
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContactId = contactId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = info.CustomerId
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<TelephonyContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_ADSL_CreateContractOnHold/StartProcess", somRequestInput);
                }


            }

            return output;
        }


        public void PostADSLSubscriptionToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
            esq.AddColumn("StContractId");
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
                var contractId = entities[0].GetColumnValue("StContractId");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<SOMAPI.ADSLSubscriptionRequestInput> somRequestInput = new SOMRequestInput<ADSLSubscriptionRequestInput>
                {

                    InputArguments = new ADSLSubscriptionRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            //ContractId = contractId.ToString(),
                            //ContactId = contactId.ToString(),
                            //AccountId = null,
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLSubscriptionRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_ADSL_CreateContract/StartProcess", somRequestInput);
                }

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
