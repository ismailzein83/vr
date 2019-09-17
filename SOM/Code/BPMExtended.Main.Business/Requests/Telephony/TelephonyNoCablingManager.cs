using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class TelephonyNoCablingManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public bool IsPhoneNumberNotExist(string phoneNumber)
        {
            bool isExist;
            using (SOMClient client = new SOMClient())
            {
                isExist = client.Get<bool>(String.Format("api/SOM.ST/Common/CheckIfPhoneNumberExist?phoneNumber={0}",phoneNumber));
            }
            return !isExist;
        }

        public SOMRequestOutput CreateTelephonyContractOnHold(Guid requestId, string coreServices, string ratePlanId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output = new SOMRequestOutput();
            List<ContractService> contractServices = new List<ContractService>();
            List<ServiceDetail> listOfCoreServices = new List<ServiceDetail>();
            string linePathId, serviceResourceId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyNoCabling");
            esq.AddColumn("StCoreServices");
            esq.AddColumn("StPathID");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StDistrict");
            esq.AddColumn("StDistrict.Id");
            esq.AddColumn("StRegion");
            esq.AddColumn("StRegion.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloorNumber");
            esq.AddColumn("StSubType");
            esq.AddColumn("StPhoneNumber");



            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                var floor = entities[0].GetColumnValue("StFloorNumber");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var street = entities[0].GetColumnValue("StStreet");
                string pathId = entities[0].GetColumnValue("StPathID").ToString();
                var subTypeId = entities[0].GetColumnValue("StSubType");
                var city = entities[0].GetColumnValue("StCityName");
                var area = entities[0].GetColumnValue("StDistrictName");
                var province = entities[0].GetColumnValue("StRegionName");
                var town = entities[0].GetColumnValue("StTownName");

                CRMCustomerInfo info = GetCRMCustomerInfo(contactId.ToString(), null);

                if (coreServices != "\"\"") listOfCoreServices = JsonConvert.DeserializeObject<List<ServiceDetail>>(coreServices);

                foreach (var item in listOfCoreServices)
                {
                    if (item.IsServiceResource) serviceResourceId = item.Id;
                }

                foreach (var item in listOfCoreServices)
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
                        PhoneNumber = phoneNumber.ToString(),
                        SubType =new CatalogManager().GetDeviceTypeIdentifierByPathId(linePathId.ToString()),
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
                    output = client.Post<SOMRequestInput<TelephonyContractOnHoldInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/CreateTelephonyContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }


            string stringifiedProcessId = output.ProcessId;
            if (output.ProcessId != null)
            {
                long processId;
                long.TryParse(stringifiedProcessId, out processId);
                var businessEntityManager = new BusinessEntityManager();
                businessEntityManager.InsertInstanceToProcessInstancesLogs(requestId, processId);
            }

            return output;

        }
        public CRMCustomerInfo GetCRMCustomerInfo(string contactId, string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            CRMCustomerInfo customerInfo = null;
            //Get infos from contact table in CRM database 

            if (contactId != null)
            {
                //PostLineSubscriptionToOM(new Guid("CAA17BBE-5D7B-4127-92B9-021417B0AC50"));
                esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
                esq.AddColumn("Name");
                esq.AddColumn("StDocumentID");
                esq.AddColumn("StCSO");
                esq.AddColumn("StCSO.Id");
                var csoBSCSIdcol = esq.AddColumn("StCSO.StCSOBSCSId");
                esq.AddColumn("StCustomerId");
                esq.AddColumn("StCustomerCategoryID");
                esq.AddColumn("StCustomerCategoryName");

                esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var csoId = entities[0].GetColumnValue("StCSOId");
                    var csoBSCSId = entities[0].GetTypedColumnValue<string>(csoBSCSIdcol.Name);
                    var name = entities[0].GetColumnValue("Name");
                    var documentId = entities[0].GetColumnValue("StDocumentID");
                    var customerId = entities[0].GetColumnValue("StCustomerId");
                    var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                    var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                    customerInfo = new CRMCustomerInfo()
                    {
                        CustomerId = customerId.ToString(),
                        CustomerName = name.ToString(),
                        DocumentID = documentId.ToString(),
                        CustomerCategoryID = customerCategoryId.ToString(),
                        CustomerCategoryName = customerCategoryName.ToString(),
                        csoId = csoId.ToString(),
                        csoBSCSId = csoBSCSId.ToString()


                    };
                }

            }
            else
            {
                //account
            }


            return customerInfo;

        }
        public ContractService ServiceDetailToContractServiceMapper(ServiceDetail item)
        {
            return new ContractService
            {
                sncode = item.Id,
                spcode = item.PackageId
            };
        }
        public bool IsContactForeigner(Guid contactId)
        {

            bool isForeigner = false;
            EntitySchemaQuery esq;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Id");
            var nationalitycol = esq.AddColumn("StCustomerDocumentType.Id");
            esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId));

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isForeigner = entities[0].GetTypedColumnValue<Guid>(nationalitycol.Name) == Guid.Parse("39A1AAE9-6FC8-4204-82B7-60E5EEE91E03") ? true : false;
            }
            return isForeigner;
        }

        public void ActivateTelephonyNoCablingToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyNoCabling");
            esq.AddColumn("StContractID");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StIsPaid");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                string fees = entities[0].GetColumnValue("StOperationAddedFees").ToString();
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                SOMRequestInput<ActivateTelephonyContractInput> somRequestInput = new SOMRequestInput<ActivateTelephonyContractInput>
                {

                    InputArguments = new ActivateTelephonyContractInput
                    {
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
                    output = client.Post<SOMRequestInput<ActivateTelephonyContractInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ActivateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);


            }


        }




    }
}
