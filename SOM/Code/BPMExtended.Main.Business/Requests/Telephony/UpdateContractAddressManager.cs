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
            esq.AddColumn("StName");
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
            esq.AddColumn("StCustomerCareer");
            esq.AddColumn("StCustomerCareer.Id");
            esq.AddColumn("StHomePhone");
            esq.AddColumn("StMobilePhone");
            esq.AddColumn("StFaxNumber");
            esq.AddColumn("StMailBox");

            esq.AddColumn("StFirstName");
            esq.AddColumn("StLastName");
            esq.AddColumn("StMinistryName");
            esq.AddColumn("StEndCustomerName");
            esq.AddColumn("StCompanyName");
            esq.AddColumn("StBranch");
            esq.AddColumn("StBusinessPhone");
            esq.AddColumn("StMiddleName");
            esq.AddColumn("StWholeSale");
            esq.AddColumn("StBusinessTypeLookup");
            esq.AddColumn("StBusinessTypeLookup.Id");


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
                var faxPhone = entities[0].GetColumnValue("StFaxNumber");
                var givenName = entities[0].GetColumnValue("StGivenName");
                var mailBox = entities[0].GetColumnValue("StMailBox");
                var languageId = entities[0].GetColumnValue("StLanguageId");
                var career = entities[0].GetColumnValue("StCustomerCareerName");

                var firstName = entities[0].GetColumnValue("StFirstName");
                var lastName = entities[0].GetColumnValue("StLastName");
                var ministryName = entities[0].GetColumnValue("StMinistryName");
                var endCustomerName = entities[0].GetColumnValue("StEndCustomerName");
                var companyName = entities[0].GetColumnValue("StCompanyName");
                var branch = entities[0].GetColumnValue("StBranch");
                var businessPhone = entities[0].GetColumnValue("StBusinessPhone");
                var middleName = entities[0].GetColumnValue("StMiddleName");
                var wholeSale = entities[0].GetColumnValue("StWholeSale");
                var businessType = entities[0].GetColumnValue("StBusinessTypeLookupName");
                var name = entities[0].GetColumnValue("StName");


                if (status.ToString() == "1") action = DirectoryInquiry.Add;
                if (status.ToString() == "2") action = DirectoryInquiry.Remove;

                SOMRequestInput<UpdateContractAddressRequestInput> somRequestInput = new SOMRequestInput<UpdateContractAddressRequestInput>
                {

                    InputArguments = new UpdateContractAddressRequestInput
                    {
                        ContractInfo = new ContractInfoDetails
                        {
                            Sequence = long.Parse(addressSeq.ToString()),
                            City = city!=null?city.ToString() : null,
                            CountryId = "206",
                            Building = buildingNumber!=null?buildingNumber.ToString():null,
                            Floor = floor!=null? floor.ToString() : null,
                            StateProvince = province!=null?province.ToString() : null,
                            Region = area!=null?area.ToString() : null,
                            Town = town!=null?town.ToString() : null,
                            Street = street!=null?street.ToString() : null,                         
                            Mailbox = mailBox!=null?mailBox.ToString() : null,
                            Career = career!=null?career.ToString() : null,
                            GivenName = givenName!=null?givenName.ToString() : null,
                            HomePhone = homePhone!=null?homePhone.ToString() : null,
                            MobilePhone = mobilePhone!=null?mobilePhone.ToString() : null,
                            FaxNumber = faxPhone!=null?faxPhone.ToString() : null,
                            Language = languageId!=null?new CRMCustomerManager().GetCustomerLanguage(languageId.ToString()) : null,
                            FirstName = firstName!=null?firstName.ToString() : null,
                            MiddleName = middleName!=null?middleName.ToString() : null,
                            LastName = lastName!=null?lastName.ToString() : null,
                            MinistryName= ministryName!=null?ministryName.ToString() : null,
                            Name= name!=null?name.ToString() : null,
                            EndCustomerName= endCustomerName!=null?endCustomerName.ToString() : null,
                            BusinessType= businessType!=null?businessType.ToString() : null,
                            BusinessPhone= businessPhone!=null?businessPhone.ToString() : null,
                            WholeSale= wholeSale!=null?wholeSale.ToString() : null,
                            CompanyName= companyName!=null?companyName.ToString() : null,
                            Branch= branch!=null?branch.ToString() : null,

                        },
                        CustomerType = new CommonManager().GetCustomerType(requestId.ToString()),
                        Action = action,
                        ServiceId = serviceId.ToString(),
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
                    output = client.Post<SOMRequestInput<UpdateContractAddressRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/UpdateContractInfo/StartProcess", somRequestInput);
                }

                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
