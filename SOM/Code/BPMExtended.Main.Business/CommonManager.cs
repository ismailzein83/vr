using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CommonManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public OutputResult ValidateRequest(string contactId, string accountId)
        {
            bool isSkip = false;
            string msgCode = "";
            ResultStatus status;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            Contact contactInfo = new Contact();
            Account accountInfo = new Account();

            //TODO:check if contact or account

            if (contactId !=null) contactInfo = GetContact(contactId);
            else accountInfo = GetAccount(accountId);

            // check categories catalog
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCustomerCategoriesInCatalog");
            esq.AddColumn("Id");
            esq.AddColumn("StSkipBalanceCheck");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerCategoryID", contactId != null? contactInfo.CustomerCategoryID : accountInfo.CustomerCategoryID);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                isSkip = (bool)entities[0].GetColumnValue("StSkipBalanceCheck");
            }

            if (isSkip)
            {
                msgCode = Constant.VALIDATION_BALANCE_VALID;
                status = ResultStatus.Success;
            }
            else
            {
                //check customer balance
                using (SOMClient client = new SOMClient())
                {
                    decimal? balance = client.Get<decimal?>(String.Format("api/SOM.ST/Billing/GetCustomerBalance?CustomerId={0}", contactId != null ? contactInfo.CustomerId : accountInfo.CustomerId));

                    if (balance == 0 || balance < 0)
                    {
                        msgCode = Constant.VALIDATION_BALANCE_VALID;
                        status = ResultStatus.Success;                        
                    }
                    else
                    {
                        msgCode = Constant.VALIDATION_BALANCE_NOT_VALID;
                        status = ResultStatus.Error;
                    }
                }               

            }

            return new OutputResult()
            {
                messages = new List<string>() { msgCode },
                status = status
            };

        }
        public string AbortRequest(string requestId)
        {
            IEntitySchemaQueryFilterItem esqFirstFilter;

            var recordSchema = BPM_UserConnection.EntitySchemaManager.GetInstanceByName("StRequestHeader");
            var recordEntity = recordSchema.CreateEntity(BPM_UserConnection);

            var eSQ = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            eSQ.AddAllSchemaColumns();
            esqFirstFilter = eSQ.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);

            eSQ.Filters.Add(esqFirstFilter);

            var collections = eSQ.GetEntityCollection(BPM_UserConnection);
            if (collections != null)
            {
                foreach (var collec in collections)
                {
                    recordEntity = collec;
                    recordEntity.SetColumnValue("StMarkAsCancelled", true);
                    recordEntity.Save();
                }
            }
            return "";
        }

        public bool IsCustomerSyrian(string contactId, string accountId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Id");
            var c = esq.AddColumn("Country");
            var country = esq.AddColumn("Country.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                var countryId = entities[0].GetColumnValue("CountryId");


                if (countryId.ToString().ToLower() == "F9EB7E62-DADB-4D0C-BC2C-38A9A33995B5".ToLower()) return true;
            }

            return false;
        }

        public string GetWorkOrderTypeName(string workOrderId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string name="";


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StStWorkOrderType1");
            esq.AddColumn("Name");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", workOrderId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                name = entities[0].GetColumnValue("Name").ToString();

            }

            return name;
        }

        public AddressData GetAddressData(string city , string province , string area, string town, string locationType)
        {
            Guid cityId=Guid.Empty, provinceId= Guid.Empty, areaId= Guid.Empty, townId= Guid.Empty, locationTypeId = Guid.Empty;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            //city
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "City");
            var IdCol = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", city);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                cityId = entities[0].GetTypedColumnValue<Guid>(IdCol.Name);
            }

            //Location Type
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLocationType");
            var IdColLocationType = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", locationType);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                locationTypeId = entities[0].GetTypedColumnValue<Guid>(IdColLocationType.Name);
            }

            //province
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Region");
            var IdColRegion = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", province);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                provinceId = entities[0].GetTypedColumnValue<Guid>(IdColRegion.Name);
            }

            //area
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StAreaLookup");
            var IdColArea = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", area);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                areaId = entities[0].GetTypedColumnValue<Guid>(IdColArea.Name);
            }

            //town
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTownLookup");
            var IdColTown = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Name", town);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                townId = entities[0].GetTypedColumnValue<Guid>(IdColTown.Name);
            }


            return new AddressData()
            {
                CityId=cityId,
                AreaId = areaId,
                ProvinceId = provinceId,
                TownId = townId,
                LocationTypeId = locationTypeId
            };
        }

        public int GetNumberOfAttachments(string schemaName , string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, schemaName+"File");
            esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, schemaName+".Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities != null)
            {
                return entities.Count;
            }

            return 0;
        }

        public Contact GetContact(string contactId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            Contact contact = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Name");
            esq.AddColumn("StDocumentID");
            esq.AddColumn("StCSO");
            esq.AddColumn("StCSO.Id");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCustomerCode");
            esq.AddColumn("StCustomerCategoryID");
            esq.AddColumn("StCustomerCategoryName"); 
            var documentType=esq.AddColumn("StDocumentIDType.Id");
            esq.AddColumn("GivenName");
            esq.AddColumn("Surname");
            esq.AddColumn("MiddleName");
            var titleId=esq.AddColumn("StTitle.Id");

            esq.AddColumn("StMotherNameTrimmed");
            esq.AddColumn("BirthDate");
            esq.AddColumn("StCareer");
            esq.AddColumn("StLanguage");
            esq.AddColumn("StLanguage.Id");
            esq.AddColumn("StNationality");
            esq.AddColumn("StNationality.Id");
            esq.AddColumn("Country");
            esq.AddColumn("Country.Id");
            esq.AddColumn("Region");
            esq.AddColumn("Region.Id");
            esq.AddColumn("City");
            esq.AddColumn("City.Id");
            esq.AddColumn("StTown");
            esq.AddColumn("StTown.Id");
            esq.AddColumn("StDistrict");
            esq.AddColumn("StDistrict.Id");
            esq.AddColumn("StBuildingNumber");
            esq.AddColumn("StFloorNumber");
            esq.AddColumn("StAddressNotes");
            esq.AddColumn("HomePhone");
            esq.AddColumn("StFaxNumber");
            esq.AddColumn("MobilePhone");
            esq.AddColumn("StBusinessPhone");
            esq.AddColumn("Email");
            esq.AddColumn("StCustomerBillCycle");
            esq.AddColumn("StBankID");
            esq.AddColumn("StBankName");
            esq.AddColumn("StBankAccountID");
            esq.AddColumn("StIBAN");
            esq.AddColumn("StStreet");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", contactId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var csoId = entities[0].GetColumnValue("StCSOId");
                var name = entities[0].GetColumnValue("Name");
                var documentId = entities[0].GetColumnValue("StDocumentID");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var customerCode = entities[0].GetColumnValue("StCustomerCode");
                var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");
                var documentIdTypeId = entities[0].GetTypedColumnValue<Guid>(documentType.Name); 
                var givenName = entities[0].GetColumnValue("GivenName");
                var surName= entities[0].GetColumnValue("Surname");
                var middleName= entities[0].GetColumnValue("MiddleName");
                var title= entities[0].GetTypedColumnValue<string>(titleId.Name);

                var motherName = entities[0].GetColumnValue("StMotherNameTrimmed");
                var birthDate = entities[0].GetColumnValue("BirthDate");
                var career = entities[0].GetColumnValue("StCareer");
                var languageId = entities[0].GetColumnValue("StLanguageId");
                var nationalityId = entities[0].GetColumnValue("StNationalityId");
                var countryId = entities[0].GetColumnValue("CountryId");
                var regionId = entities[0].GetColumnValue("RegionId");
                var cityId = entities[0].GetColumnValue("CityId");
                var townId = entities[0].GetColumnValue("StTownId");
                var districtId = entities[0].GetColumnValue("StDistrictId");
                var buildingNumber = entities[0].GetColumnValue("StBuildingNumber");
                var floorNumber = entities[0].GetColumnValue("StFloorNumber");
                var addressNotes = entities[0].GetColumnValue("StAddressNotes");
                var homePhone = entities[0].GetColumnValue("HomePhone");
                var faxNumber = entities[0].GetColumnValue("StFaxNumber");
                var mobilePhone = entities[0].GetColumnValue("MobilePhone");
                var businessPhone = entities[0].GetColumnValue("StBusinessPhone");
                var email = entities[0].GetColumnValue("Email");
                var customerBillCycle = entities[0].GetColumnValue("StCustomerBillCycle");
                var bankID = entities[0].GetColumnValue("StBankID");
                var bankName = entities[0].GetColumnValue("StBankName");
                var bankAccountID = entities[0].GetColumnValue("StBankAccountID");
                var IBAN = entities[0].GetColumnValue("StIBAN");
                var street = entities[0].GetColumnValue("StStreet");


                contact = new Contact()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString(),
                    DocumentID = documentId.ToString(),
                    CustomerCategoryID = customerCategoryId.ToString(),
                    CustomerCategoryName = customerCategoryName.ToString(),
                    CSOId = csoId.ToString(),
                    CustomerCode = customerCode.ToString(),
                    DocumentIdTypeId= documentIdTypeId.ToString(),
                    GivenName=givenName.ToString(),
                    SurName=surName.ToString(),
                    MiddleName=middleName.ToString(),
                    Title=title.ToString(),
                    MotherName = motherName.ToString(),
                    BirthDate = birthDate.ToString(),
                    Career = career.ToString(),
                    LanguageId = languageId != null? languageId.ToString(): null,
                    NationalityId = nationalityId.ToString(),
                    CountryId = countryId.ToString(),
                    RegionId = regionId.ToString(),
                    CityId = cityId.ToString(),
                    TownId = townId.ToString(),
                    DistrictId = districtId.ToString(),
                    BuildingNumber = buildingNumber.ToString(),
                    FloorNumber = floorNumber.ToString(),
                    AddressNotes = addressNotes.ToString(),
                    HomePhone = homePhone.ToString(),
                    FaxNumber = faxNumber.ToString(),
                    MobilePhone = mobilePhone.ToString(),
                    BusinessPhone = businessPhone.ToString(),
                    Email = email.ToString(),
                    CustomerBillCycle = customerBillCycle.ToString(),  
                    BankID= bankID.ToString(),
                    BankName = bankName.ToString(),
                    BankAccountID = bankAccountID.ToString(),
                    IBAN = IBAN.ToString(),
                    Street = street.ToString()

                    
                };
            }
            return contact;
        }

        public Account GetAccount(string accountId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            Account account = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Account");
            esq.AddColumn("Name");
            esq.AddColumn("StCSO");
            esq.AddColumn("StCSO.Id");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StCustomerCode");
            esq.AddColumn("StCustomerCategoryID");
            esq.AddColumn("StCustomerCategoryName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", accountId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var csoId = entities[0].GetColumnValue("StCSOId");
                var name = entities[0].GetColumnValue("Name");
                var customerId = entities[0].GetColumnValue("StCustomerId");
                var customerCode = entities[0].GetColumnValue("StCustomerCode");
                var customerCategoryId = entities[0].GetColumnValue("StCustomerCategoryID");
                var customerCategoryName = entities[0].GetColumnValue("StCustomerCategoryName");

                account = new Account()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString(),
                    CustomerCategoryID = customerCategoryId.ToString(),
                    CustomerCategoryName = customerCategoryName.ToString(),
                    CSOId = csoId.ToString(),
                    CustomerCode = customerCode.ToString()


                };
            }
            return account;
        }

        public void UpdateDepositDocumentId(string requestId, List<DepositDocument> depositDocument)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<DepositDocument> listOfDeposites = new List<DepositDocument>();

            string entityName = new CRMCustomerManager().GetEntityNameByRequestId(requestId);

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            esq.AddColumn("StOperationAddedDeposites");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                string deposites = entities[0].GetColumnValue("StOperationAddedDeposites").ToString();

                if (deposites != "\"\"" && deposites != null && deposites != "")
                {
                    listOfDeposites = JsonConvert.DeserializeObject<List<DepositDocument>>(deposites);
                }

                List<DepositDocument> newDepositsList = listOfDeposites.Concat(depositDocument).ToList();


                //update deposites
                UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var update = new Update(connection, entityName).Set("StOperationAddedDeposites", Column.Parameter(JsonConvert.SerializeObject(newDepositsList)))
                    .Where("Id").IsEqual(Column.Parameter(requestId));
                update.Execute();


            }

        }

        public List<Contact> GetActiveContacts()
        {
            using (SOMClient client = new SOMClient())
            {
                return client.Get<List<Contact>>(String.Format("api/SOM.ST/Billing/GetActiveContacts"));
            }
        }

        public List<Account> GetActiveAccounts()
        {
            using (SOMClient client = new SOMClient())
            {
                return client.Get<List<Account>>(String.Format("api/SOM.ST/Billing/GetActiveAccounts"));
            }

        }

        public List<Contact> GetValidContacts()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter, esqSecondFilter;
            List<Contact> contacts = new List<Contact>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Contact");
            esq.AddColumn("Name");
            esq.AddColumn("StBlackList");
            esq.AddColumn("StCustomerId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StBlackList", false);
            esqSecondFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StCustomerType.Id", "6C906EA6-B0FC-478F-A463-D94CB7E54F6D");
            esq.Filters.Add(esqFirstFilter);
            esq.Filters.Add(esqSecondFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                var customerId = entity.GetColumnValue("StCustomerId");
                var name = entity.GetColumnValue("Name");

                contacts.Add(new Contact()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString()
                });
            }
            return contacts;
        }

        public List<Account> GetValidAccounts()
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            List<Account> accounts = new List<Account>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "Account");
            esq.AddColumn("Name");
            esq.AddColumn("StBlackList");
            esq.AddColumn("StCustomerId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StBlackList", false);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            foreach (Entity entity in entities)
            {
                var customerId = entity.GetColumnValue("StCustomerId");
                var name = entity.GetColumnValue("Name");

                accounts.Add(new Account()
                {
                    CustomerId = customerId.ToString(),
                    CustomerName = name.ToString()
                });
            }
            return accounts;
        }

        public bool IsOperationExistInRequestHeader(string contractId, string requestType, string statusId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2, esqFilter3;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", requestType);
            esqFilter3 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus", statusId);

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);
            esq.Filters.Add(esqFilter3);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsContractHasAnyOpenOperation(string contractId)
        {
            if (contractId == null) return false;

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            var status = esq.AddColumn("StStatus.StIsClosed");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus.StIsClosed", false);

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                //return true;
                return false; // to be removed and must be replaced by the above one

            }
            return false;
        }




        public List<string> GetOperationsFromRequestHeader(string requestType, string statusId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFilter, esqFilter2;
            List<string> requestIds = new List<string>();

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("Id");
            esq.AddColumn("StRequestId");

            esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestType", requestType);
            esqFilter2 = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStatus", statusId);

            esq.Filters.Add(esqFilter);
            esq.Filters.Add(esqFilter2);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            for (int i = 0; i < entities.Count; i++)
            {
                var id = entities[i].GetColumnValue("StRequestId");
                requestIds.Add(id.ToString());
            }
            return requestIds;
        }
        public string GetEntityNameByRequestId(string requestId)
        {
            
            string operationId = GetOperationByRequestId(requestId);

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string entityName = "";


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestsTypes");
            esq.AddColumn("StEntityName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", operationId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                entityName = entities[0].GetColumnValue("StEntityName").ToString();

            }
            return entityName;
        }

        public string GetEntityNameByOperationId(string operationId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string entityName = "";


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestsTypes");
            esq.AddColumn("StEntityName");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", operationId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                entityName = entities[0].GetColumnValue("StEntityName").ToString();

            }
            return entityName;
        }

        public string GetStepColumnNameByRequestId(string requestId)
        {

            string operationId = GetOperationByRequestId(requestId);

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string stepColumnName = "";


            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestsTypes");
            esq.AddColumn("StStepColumn");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", operationId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                stepColumnName = entities[0].GetColumnValue("StStepColumn").ToString();

            }
            return stepColumnName;
        }

        public string GetCompletedStepIdByEntityName(string entityName)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string completedStepId = null;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestsTypes");
            esq.AddColumn("StCompletedStepId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StEntityName", entityName);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                completedStepId = entities[0].GetColumnValue("StCompletedStepId").ToString();

            }
            return completedStepId;
        }

        public bool IsRequestAdvantageous(string entityName , string requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            bool isAdvantageous = false;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, entityName);
            esq.AddColumn("StAdvantageous");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                isAdvantageous = (bool)entities[0].GetColumnValue("StAdvantageous");

            }
            return isAdvantageous;
        }

        public Guid GetRequestStageIdByStepId(string stepId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            Guid requestStageId = Guid.Empty;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequeststage");
            var IdCol = esq.AddColumn("Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StStageId", stepId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                requestStageId = entities[0].GetTypedColumnValue<Guid>(IdCol.Name);

            }
            return requestStageId;
        }

        public string GetOperationByRequestId(string requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string operationId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StOperation");
            esq.AddColumn("StOperation.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StRequestId", requestId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                operationId = entities[0].GetColumnValue("StOperationId").ToString();

            }
            return operationId;
        }

        public string GetOperationByContractId(string contractId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string operationId = "";

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StRequestHeader");
            esq.AddColumn("StOperation");
            esq.AddColumn("StOperation.Id");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StContractID", contractId);
            esq.Filters.Add(esqFirstFilter);
            entities = esq.GetEntityCollection(BPM_UserConnection);

            if (entities.Count > 0)
            {
                operationId = entities[0].GetColumnValue("StOperationId").ToString();

            }
            return operationId;
        }

    }
}
