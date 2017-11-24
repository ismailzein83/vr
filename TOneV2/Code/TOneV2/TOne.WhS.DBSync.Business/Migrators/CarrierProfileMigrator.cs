using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;
using System.IO;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.DBSync.Business
{
    class VRTimeZoneByProfile
    {
        public VRTimeZoneByProfile()
        {
            this.CustomerVRTimeZones = new List<VRTimeZone>();
            this.SupplierVRTimeZones = new List<VRTimeZone>();
        }
        public int ProfileId { get; set; }
        public List<VRTimeZone> CustomerVRTimeZones { get; set; }
        public List<VRTimeZone> SupplierVRTimeZones { get; set; }
    }
    public class CarrierProfileMigrator : Migrator<SourceCarrierProfile, CarrierProfile>
    {
        Dictionary<string, CarrierProfileBlockedStatusEntity> InActiveProfileIds;
        CarrierProfileDBSyncDataManager dbSyncDataManager;
        SourceCarrierProfileDataManager dataManager;
        SourceCarrierDocumentDataManager carrierDocumentDataManager;
        FileDBSyncDataManager fileDataManager;
        Dictionary<string, Country> allCountries;
        Dictionary<string, Currency> allCurrencies;
        Dictionary<string, VRTimeZone> allTimeZones;
        Dictionary<string, List<SourceCarrierDocument>> allCarrierDocumentsByProfileId;
        Dictionary<string, VRTimeZoneByProfile> _timeZonesByProfile;
        BusinessEntityTechnicalSettingsData bETechnicalSettingsData;

        public CarrierProfileMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierProfileDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCarrierProfileDataManager(Context.ConnectionString);
            fileDataManager = new FileDBSyncDataManager(context.UseTempTables);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCountry = Context.DBTables[DBTableName.Country];
            allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            carrierDocumentDataManager = new SourceCarrierDocumentDataManager(Context.ConnectionString);

            var dbTableVRTimeZone = context.DBTables[DBTableName.VRTimeZone];
            allTimeZones = (Dictionary<String, VRTimeZone>)dbTableVRTimeZone.Records;

            _timeZonesByProfile = GetTimeZonesByProfile(new SourceCarrierAccountDataManager(Context.ConnectionString).GetTimeZonesByProfile());

            allCarrierDocumentsByProfileId = GetAccountDocumentsAndMigrateBETechnicalSettings();

            InActiveProfileIds = dataManager.GetProfileIdsWithNoActiveAccounts();
        }

        Dictionary<string, VRTimeZoneByProfile> GetTimeZonesByProfile(List<TimeZonesByProfile> list)
        {
            Dictionary<string, VRTimeZoneByProfile> result = new Dictionary<string, VRTimeZoneByProfile>();

            foreach (var sourceTimeZone in list)
            {
                VRTimeZoneByProfile vrTimeZoneByProfile = result.GetOrCreateItem(sourceTimeZone.CarrierProfileId.ToString());

                if (allTimeZones.ContainsKey(sourceTimeZone.CustomerTimeZoneId.ToString()))
                    vrTimeZoneByProfile.CustomerVRTimeZones.Add(allTimeZones[sourceTimeZone.CustomerTimeZoneId.ToString()]);
                if (allTimeZones.ContainsKey(sourceTimeZone.SupplierTimeZoneId.ToString()))
                    vrTimeZoneByProfile.SupplierVRTimeZones.Add(allTimeZones[sourceTimeZone.SupplierTimeZoneId.ToString()]);
            }

            return result;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<CarrierProfile> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierProfilesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
            Dictionary<string, CarrierProfile> carrierProfiles = dbSyncDataManager.GetCarrierProfiles(true);
            var carrierProfileManager = new CarrierProfileManager();
            var fileManager = new VRFileManager();

            foreach (var carrierProfile in carrierProfiles.Values)
            {
                List<long> fileIds = new List<long>();
                if (carrierProfile.Settings != null && carrierProfile.Settings.Documents != null && carrierProfile.Settings.Documents.Count > 0)
                {
                    fileIds = carrierProfile.Settings.Documents.Select(itm => itm.FileId).ToList();
                }
                if (carrierProfile.Settings != null && carrierProfile.Settings.CompanyLogo.HasValue)
                    fileIds.Add(carrierProfile.Settings.CompanyLogo.Value);

                carrierProfileManager.SetFilesUsed(fileIds, carrierProfile.CarrierProfileId);
            }
        }

        public override IEnumerable<SourceCarrierProfile> GetSourceItems()
        {
            return dataManager.GetSourceCarrierProfiles();
        }

        public override CarrierProfile BuildItemFromSource(SourceCarrierProfile sourceItem)
        {
            int? countryId = null;

            Country country = null;

            if (allCountries != null && !string.IsNullOrWhiteSpace(string.Empty))
                country = allCountries.Values.Where(x => x.Name == sourceItem.Country).FirstOrDefault();
            if (country != null)
            {
                countryId = country.CountryId;
            }

            Currency currency = null;
            if (allCurrencies != null && sourceItem.CurrencyId != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId.ToString(), out currency);

            CarrierProfileSettings settings = new CarrierProfileSettings();

            settings.SupplierActivationStatus = CarrierProfileActivationStatus.Active;
            settings.CustomerActivationStatus = CarrierProfileActivationStatus.Active;

            CarrierProfileBlockedStatusEntity carrierProfileBlockedEntity;
            if (InActiveProfileIds.TryGetValue(sourceItem.SourceId, out carrierProfileBlockedEntity))
            {
                settings.SupplierActivationStatus = carrierProfileBlockedEntity.IsSupplierBlocked ? CarrierProfileActivationStatus.InActive : CarrierProfileActivationStatus.Active;
                settings.CustomerActivationStatus = carrierProfileBlockedEntity.IsCustomerBlocked ? CarrierProfileActivationStatus.InActive : CarrierProfileActivationStatus.Active;
            }

            settings.Address = sourceItem.Address1;
            settings.PostalCode = sourceItem.Address2;
            settings.Town = sourceItem.Address3;
            settings.Company = sourceItem.CompanyName;
            settings.RegistrationNumber = sourceItem.RegistrationNumber;
            settings.Website = sourceItem.Website;

            if (currency != null)
                settings.CurrencyId = currency.CurrencyId;

            List<SourceCarrierDocument> carrierDocuments;
            if (allCarrierDocumentsByProfileId.TryGetValue(sourceItem.SourceId, out carrierDocuments))
            {
                List<VRDocumentSetting> profileDocuments = new List<VRDocumentSetting>();
                foreach (SourceCarrierDocument carrierDocumet in carrierDocuments)
                {
                    string documentExtension = Path.GetExtension(carrierDocumet.Name);
                    if (!string.IsNullOrEmpty(documentExtension))
                        documentExtension = documentExtension.Substring(1);

                    VRFile document = new VRFile()
                    {
                        Content = carrierDocumet.Document,
                        Extension = documentExtension,
                        CreatedTime = carrierDocumet.Created,
                        Name = carrierDocumet.Name,
                        IsTemp = false,
                    };


                    long documentId = fileDataManager.ApplyFile(document);

                    profileDocuments.Add(new VRDocumentSetting()
                    {
                        CategoryId = bETechnicalSettingsData.DocumentCategoryDefinition.ItemDefinitions.FindRecord(item => item.Title.Equals(carrierDocumet.Category, StringComparison.InvariantCultureIgnoreCase)).ItemId,
                        CreatedOn = carrierDocumet.Created,
                        Description = carrierDocumet.Description,
                        FileId = documentId
                    });
                }

                settings.Documents = profileDocuments;
            }


            List<CarrierContact> contacts = new List<CarrierContact>();
            contacts.Add(new CarrierContact { Description = sourceItem.AccountManagerContact, Type = CarrierContactType.AccountManagerContact });
            contacts.Add(new CarrierContact { Description = sourceItem.AccountManagerEmail, Type = CarrierContactType.AccountManagerEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.SMSPhoneNumber, Type = CarrierContactType.AlertingSMSPhoneNumbers });
            contacts.Add(new CarrierContact { Description = sourceItem.BillingContact, Type = CarrierContactType.BillingContactPerson });
            contacts.Add(new CarrierContact { Description = sourceItem.BillingEmail, Type = CarrierContactType.BillingEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.CommercialContact, Type = CarrierContactType.CommercialContactPerson });
            contacts.Add(new CarrierContact { Description = sourceItem.CommercialEmail, Type = CarrierContactType.CommercialEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.BillingDisputeEmail, Type = CarrierContactType.DisputeEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.PricingContact, Type = CarrierContactType.PricingContactPerson });
            contacts.Add(new CarrierContact { Description = sourceItem.PricingEmail, Type = CarrierContactType.PricingEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.SupportContact, Type = CarrierContactType.SupportContactPerson });
            contacts.Add(new CarrierContact { Description = sourceItem.SupportEmail, Type = CarrierContactType.SupportEmail });
            contacts.Add(new CarrierContact { Description = sourceItem.TechnicalContact, Type = CarrierContactType.TechnicalContactPerson });
            contacts.Add(new CarrierContact { Description = sourceItem.TechnicalEmail, Type = CarrierContactType.TechnicalEmail });

            string[] stringSeparators = new string[] { "\r\n" };

            List<string> faxes = new List<string>();
            if (sourceItem.Fax != null)
                faxes = sourceItem.Fax.Split(stringSeparators, StringSplitOptions.None).Where(x => x != string.Empty).ToList();


            List<string> phoneNumbers = new List<string>();
            if (sourceItem.Telephone != null)
                phoneNumbers = sourceItem.Telephone.Split(stringSeparators, StringSplitOptions.None).Where(x => x != string.Empty).ToList();

            settings.Contacts = contacts;
            settings.Faxes = faxes;
            settings.PhoneNumbers = phoneNumbers;
            VRTimeZoneByProfile vrTimeZoneByProfile;
            if (!_timeZonesByProfile.TryGetValue(sourceItem.SourceId, out vrTimeZoneByProfile))
            {
                settings.DefaultCusotmerTimeZoneId = allTimeZones.First().Value.TimeZoneId;
                settings.DefaultSupplierTimeZoneId = allTimeZones.First().Value.TimeZoneId;
            }
            else
            {
                settings.DefaultCusotmerTimeZoneId = vrTimeZoneByProfile.CustomerVRTimeZones.GroupBy(t => t.TimeZoneId).Select(t => new { VRTimeZoneId = t.Key, Count = t.Count() }).OrderByDescending(t => t.Count).First().VRTimeZoneId;
                settings.DefaultSupplierTimeZoneId = vrTimeZoneByProfile.SupplierVRTimeZones.GroupBy(t => t.TimeZoneId).Select(t => new { VRTimeZoneId = t.Key, Count = t.Count() }).OrderByDescending(t => t.Count).First().VRTimeZoneId;
            }
            if (sourceItem.CompanyLogo != null)
            {
                string[] nameastab = sourceItem.CompanyLogoName.Split('.');
                VRFile file = new VRFile()
                {
                    Content = sourceItem.CompanyLogo,
                    Name = sourceItem.CompanyLogoName,
                    Extension = nameastab[nameastab.Length - 1],
                    CreatedTime = DateTime.Now,
                    IsTemp = false,
                };

                settings.CompanyLogo = fileDataManager.ApplyFile(file);
            }

            settings.CountryId = countryId;

            return new CarrierProfile
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId,
                Settings = settings,
                IsDeleted = sourceItem.IsDeleted
            };


        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            if (dbTableCarrierProfile != null)
                dbTableCarrierProfile.Records = dbSyncDataManager.GetCarrierProfiles(useTempTables);
        }


        #region Private Methods

        private Dictionary<string, List<SourceCarrierDocument>> GetAccountDocumentsAndMigrateBETechnicalSettings()
        {
            Dictionary<string, List<SourceCarrierDocument>> carrierDocumentsByProfileId = new Dictionary<string, List<SourceCarrierDocument>>();
            IEnumerable<SourceCarrierDocument> sourceCarrierDocuments = carrierDocumentDataManager.GetSourceCarrierDocuments();
            if (sourceCarrierDocuments != null)
            {
                List<SourceCarrierDocument> carrierDocumentsList;
                foreach (SourceCarrierDocument sourceCarrierDocument in sourceCarrierDocuments)
                {
                    if (!carrierDocumentsByProfileId.TryGetValue(sourceCarrierDocument.ProfileId.ToString(), out carrierDocumentsList))
                    {
                        carrierDocumentsList = new List<SourceCarrierDocument>();
                        carrierDocumentsByProfileId.Add(sourceCarrierDocument.ProfileId.ToString(), carrierDocumentsList);
                    }

                    carrierDocumentsList.Add(sourceCarrierDocument);
                }
            }

            bETechnicalSettingsData = MigrateBETechnicalSettings(sourceCarrierDocuments);

            return carrierDocumentsByProfileId;
        }

        private BusinessEntityTechnicalSettingsData MigrateBETechnicalSettings(IEnumerable<SourceCarrierDocument> sourceCarrierDocuments)
        {
            SettingManager settingManager = new SettingManager();
            Setting bETechnicalSettings = settingManager.GetSettingByType("WhS_BE_TechnicalSettings");
            BusinessEntityTechnicalSettingsData bETechnicalSettingsData = bETechnicalSettings.Data as BusinessEntityTechnicalSettingsData;

            if (sourceCarrierDocuments != null)
            {

                if (bETechnicalSettings != null)
                {
                    VRDocumentCategoryDefinition documentDefinition = new VRDocumentCategoryDefinition();
                    documentDefinition.ItemDefinitions = new List<VRDocumentItemDefinition>();
                    IEnumerable<string> distinctDocumentsCategories = sourceCarrierDocuments.Select(item => item.Category).Distinct(StringComparer.InvariantCultureIgnoreCase);
                    if (distinctDocumentsCategories != null)
                    {
                        foreach (string documentCategory in distinctDocumentsCategories)
                        {
                            VRDocumentItemDefinition documentItemDefinition = new VRDocumentItemDefinition()
                            {
                                ItemId = Guid.NewGuid(),
                                Title = documentCategory
                            };

                            documentDefinition.ItemDefinitions.Add(documentItemDefinition);
                        }
                    }

                    bETechnicalSettingsData.DocumentCategoryDefinition = documentDefinition;
                    SettingToEdit settingToEdit = new SettingToEdit()
                    {
                        SettingId = bETechnicalSettings.SettingId,
                        Name = bETechnicalSettings.Name,
                        Data = bETechnicalSettings.Data,
                    };
                    settingManager.UpdateSetting(settingToEdit);
                }
            }

            return bETechnicalSettingsData;
        }

        #endregion
    }
}
