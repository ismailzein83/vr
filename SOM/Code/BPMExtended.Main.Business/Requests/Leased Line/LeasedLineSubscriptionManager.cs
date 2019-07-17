using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LeasedLineSubscriptionManager
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

        public SOMRequestOutput CreateTelephonyContractOnHold(Guid requestId, string coreServices, string optionalServices, string ratePlanId)
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
                    output = client.Post<SOMRequestInput<TelephonyContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateTelephonyContract/StartProcess", somRequestInput);
                }


            }

            return output;
        }

        public void PostLeasedLineRequestToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");
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

                SOMRequestInput<LeasedLineRequestInput> somRequestInput = new SOMRequestInput<LeasedLineRequestInput>
                {

                    InputArguments = new LeasedLineRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            //ContractId = contractId.ToString(),
                            //ContactId = contactId.ToString(),
                            // AccountId = null,
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LeasedLineRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_LL_CreateContract/StartProcess", somRequestInput);
                }

            }

        }

        public string GetConnectionType(string requestId)
        {
            Random random = new Random();
            return random.Next(10)>5?"Fiber":"Copper";
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
