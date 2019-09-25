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

        public SOMRequestOutput CreateLeasedLineContractOnHold(Guid requestId, string coreServices, string optionalServices, string ratePlanId)
        {

            //Get Data from StLeasedLine table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output = new SOMRequestOutput();
            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            List<ServiceDetail> listOfOptionalServices = new List<ServiceDetail>();
            List<DepositDocument> depositServices = new List<DepositDocument>();
            string linePathId, serviceResourceId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StServices");
            esq.AddColumn("StLinePathID");
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
            esq.AddColumn("StAddressNotes");
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
                var locationType = entities[0].GetColumnValue("StLocationName");
                var addressNotes = entities[0].GetColumnValue("StAddressNotes");

                CRMCustomerInfo info = new CRMCustomerManager().GetCRMCustomerInfo(contactId.ToString(), null);

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
                SOMRequestInput<LeasedLineContractOnHoldInput> somRequestInput = new SOMRequestInput<LeasedLineContractOnHoldInput>
                {
                    InputArguments = new LeasedLineContractOnHoldInput
                    {
                        LinePathId = linePathId,
                        SubType = "LeaseLine",
                        ServiceResource = serviceResourceId,
                        City = city.ToString(),
                        Building = buildingNumber.ToString(),
                        Floor = floor.ToString(),
                        Town = town.ToString(),
                        StateProvince = province.ToString(),
                        Street = street.ToString(),
                        Region = area.ToString(),
                        CountryId = "206",
                        CSO = info.csoBSCSId,//info.csoId,
                        RatePlanId = ratePlanId,//ratePlanId.ToString(),
                        ContractServices = contractServices,
                        DepositServices = depositServices,
                        LocationType = locationType.ToString(),
                        Notes = addressNotes.ToString(),
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContactId = contactId.ToString(),
                            RequestId = requestId.ToString(),
                            CustomerId = info.CustomerId //"CusId00026"
                        }
                    }

                };

                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LeasedLineContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateLeaseLineContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


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
            esq.AddColumn("StContractID");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
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
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }

        }

        public string GetConnectionType(string requestId)
        {
            Random random = new Random();
            return random.Next(10)>5?"Fiber":"Copper";
        }

        public int CheckLeasedLineContractOnHoldStatus(string processId)
        {
            int status;

            using (SOMClient client = new SOMClient())
            {
                status = client.Get<int>(String.Format("api/SOM.ST/Common/CheckWorkflowStatus?processInstanceId={0}", processId));
            }

            return status;

            //Array values = Enum.GetValues(typeof(ContacrtOnHoldStatus));
            //Random random = new Random();
            //int randomBar = (int)values.GetValue(random.Next(values.Length));
            //return randomBar;

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
