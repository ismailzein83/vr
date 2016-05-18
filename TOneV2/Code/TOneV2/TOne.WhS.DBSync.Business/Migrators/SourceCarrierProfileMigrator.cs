using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierProfileMigrator : Migrator<SourceCarrierProfile, CarrierProfile>
    {
        CarrierProfileDBSyncDataManager dbSyncDataManager;
        SourceCarrierProfileDataManager dataManager;

        public SourceCarrierProfileMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierProfileDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceCarrierProfileDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CarrierProfile> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierProfilesToTemp(itemsToAdd);
            DBTable dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            if (dbTableCarrierProfile != null)
                dbTableCarrierProfile.Records = dbSyncDataManager.GetCarrierProfiles();
        }

        public override IEnumerable<SourceCarrierProfile> GetSourceItems()
        {
            return dataManager.GetSourceCarrierProfiles();
        }

        public override CarrierProfile BuildItemFromSource(SourceCarrierProfile sourceItem)
        {
            VRFileManager vrFileManager = new VRFileManager();

            int? countryId = null;
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null)
            {
                Dictionary<string, Country> allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
                Country country = null;
                if (allCountries != null && !string.IsNullOrWhiteSpace(string.Empty))
                    country = allCountries.Values.Where(x => x.Name == sourceItem.Country).FirstOrDefault();
                if (country != null)
                {
                    countryId = country.CountryId;
                }
            }


            CarrierProfileSettings settings = new CarrierProfileSettings();
            settings.Address = sourceItem.Address1;
            settings.PostalCode = sourceItem.Address2;
            settings.Town = sourceItem.Address3;
            settings.Company = sourceItem.CompanyName;
            settings.RegistrationNumber = sourceItem.RegistrationNumber;
            settings.Website = sourceItem.Website;

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

            if (sourceItem.CompanyLogo != null)
            {
                string[] nameastab = sourceItem.CompanyLogoName.Split('.');
                VRFile file = new VRFile()
                {
                    Content = sourceItem.CompanyLogo,
                    Name = sourceItem.CompanyLogoName,
                    Extension = nameastab[nameastab.Length - 1],
                    CreatedTime = DateTime.Now

                };

                settings.CompanyLogo = vrFileManager.AddFile(file);
            }

            settings.CountryId = countryId;

            return new CarrierProfile
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId,
                Settings = settings
            };


        }
    }
}
