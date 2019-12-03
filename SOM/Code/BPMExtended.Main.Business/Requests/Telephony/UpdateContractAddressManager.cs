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
    public class UpdateContractAddressManager
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
        public void PostUpdateContractAddressToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;
            DirectoryInquiry action = DirectoryInquiry.NoAction;
            string serviceId = "";
            EntityCollection entities;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("StPublicDIId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "E7CF42F9-7A83-4AD2-A73A-5203C94A4DA2");
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                serviceId = entities[0].GetColumnValue("StPublicDIId").ToString();
            }

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StUpdateContractAddress");
            esq.AddColumn("StContractID");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StDirectoryStatus");
            esq.AddColumn("StStreet");
            esq.AddColumn("StAddressSequence");
            esq.AddColumn("StOperationAddedFees");
            esq.AddColumn("StOperationAddedDeposites");
            esq.AddColumn("StOperationAddedServices");
            esq.AddColumn("StIsPaid");
            esq.AddColumn("StCity");
            esq.AddColumn("StCity.Id");
            esq.AddColumn("StArea");
            esq.AddColumn("StArea.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StProvince");
            esq.AddColumn("StProvince.Id");
            esq.AddColumn("StLocation");
            esq.AddColumn("StLocation.Id");
            esq.AddColumn("StStreet");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloor");

            esq.AddColumn("StGivenName");
            esq.AddColumn("StLanguage");
            esq.AddColumn("StLanguage.Id");
            esq.AddColumn("StCareer");
            esq.AddColumn("StCareer.Id");
            esq.AddColumn("StHomePhone");
            esq.AddColumn("StMobilePhone");
            esq.AddColumn("StFaxPhone");
            esq.AddColumn("StMailBox");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var street = entities[0].GetColumnValue("StStreet");
                var addressSeq = entities[0].GetColumnValue("StAddressSequence");
                var status = entities[0].GetColumnValue("StDirectoryStatus");
                var city = entities[0].GetColumnValue("StCityName");
                var town = entities[0].GetColumnValue("StTownName");
                var area = entities[0].GetColumnValue("StAreaName");
                var province = entities[0].GetColumnValue("StProvinceName");
                var locationType = entities[0].GetColumnValue("StLocationName");
                var floor = entities[0].GetColumnValue("StFloor");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var fees = entities[0].GetColumnValue("StOperationAddedFees");
                var deposits = entities[0].GetColumnValue("StOperationAddedDeposites");
                var services = entities[0].GetColumnValue("StOperationAddedServices");
                var isPaid = entities[0].GetColumnValue("StIsPaid");

                var homePhone = entities[0].GetColumnValue("StHomePhone");
                var mobilePhone = entities[0].GetColumnValue("StMobilePhone");
                var faxPhone = entities[0].GetColumnValue("StFaxPhone");
                var givenName = entities[0].GetColumnValue("StGivenName");
                var mailBox = entities[0].GetColumnValue("StMailBox");
                var languageId = entities[0].GetColumnValue("StLanguageId");
                var career = entities[0].GetColumnValue("StCareerName");


                if (status.ToString() == "1") action = DirectoryInquiry.Add;
                if (status.ToString() == "2") action = DirectoryInquiry.Remove;

                SOMRequestInput<UpdateContractAddressRequestInput> somRequestInput = new SOMRequestInput<UpdateContractAddressRequestInput>
                {

                    InputArguments = new UpdateContractAddressRequestInput
                    {
                        AddressSequence = addressSeq.ToString(),
                        City = city.ToString(),
                        CountryId = "206",
                        Building = buildingNumber.ToString(),
                        Floor = floor.ToString(),
                        StateProvince = province.ToString(),
                        Region = area.ToString(),
                        Town = town.ToString(),
                        ServiceId = serviceId.ToString(),
                        LocationType = locationType.ToString(),
                        Street = street.ToString(),
                        Action = action,
                        Mailbox = mailBox.ToString(),
                        Career = career.ToString(),
                        GivenName = givenName.ToString(),
                        HomePhone = homePhone.ToString(),
                        MobilePhone = mobilePhone.ToString(),
                        FaxNumber = faxPhone.ToString(),
                        Language = new CRMCustomerManager().GetCustomerLanguage(languageId.ToString()),
                        PaymentData = new PaymentData()
                        {
                            Fees = JsonConvert.DeserializeObject<List<SaleService>>(fees.ToString()),
                            //Services = JsonConvert.DeserializeObject<List<VASService>>(services.ToString()),
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
                    output = client.Post<SOMRequestInput<UpdateContractAddressRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Billing_UpdateContractAddressAndAddToPublicDirectory/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
