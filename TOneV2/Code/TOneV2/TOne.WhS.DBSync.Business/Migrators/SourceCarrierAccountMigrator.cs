using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using System;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierAccountMigrator : Migrator<SourceCarrierAccount, CarrierAccount>
    {
        CarrierAccountDBSyncDataManager dbSyncDataManager;
        SourceCarrierAccountDataManager dataManager;

        public SourceCarrierAccountMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierAccountDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceCarrierAccountDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CarrierAccount> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierAccountsToTemp(itemsToAdd);
        }

        public override IEnumerable<SourceCarrierAccount> GetSourceItems()
        {
            return dataManager.GetSourceCarrierAccounts();
        }

        public override CarrierAccount BuildItemFromSource(SourceCarrierAccount sourceItem)
        {
            CarrierAccountSettings carrierAccountSettings = new CarrierAccountSettings();
            CarrierAccountCustomerSettings carrierAccountCustomerSettings = new CarrierAccountCustomerSettings();
            CarrierAccountSupplierSettings carrierAccountSupplierSettings = new CarrierAccountSupplierSettings();

            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            DBTable dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            if (dbTableCarrierProfile != null && dbTableCurrency != null)
            {
                List<CarrierProfile> allCarrierProfiles = (List<CarrierProfile>)dbTableCarrierProfile.Records;
                CarrierProfile carrierProfile = null;
                if (allCarrierProfiles != null)
                    carrierProfile = allCarrierProfiles.Where(x => x.SourceId == sourceItem.ProfileId.ToString()).FirstOrDefault();

                List<Currency> allCurrencies = (List<Currency>)dbTableCurrency.Records;

                Currency currency = null;
                if (allCurrencies != null)
                    currency = allCurrencies.Where(x => x.SourceId == sourceItem.CurrencyID).FirstOrDefault();

                if (carrierProfile != null && currency != null)
                {
                    carrierAccountSettings.CurrencyId = currency.CurrencyId;

                    switch (sourceItem.ActivationStatus)
                    {
                        case SourceActivationStatus.Active:
                            carrierAccountSettings.ActivationStatus = ActivationStatus.Active;
                            break;

                        case SourceActivationStatus.Inactive:
                            carrierAccountSettings.ActivationStatus = ActivationStatus.Inactive;
                            break;

                        case SourceActivationStatus.Testing:
                            carrierAccountSettings.ActivationStatus = ActivationStatus.Testing;
                            break;
                    }

                    CarrierAccountType accountType;

                    switch (sourceItem.AccountType)
                    {
                        case SourceAccountType.Client:
                            accountType = CarrierAccountType.Customer;
                            break;

                        case SourceAccountType.Exchange:
                            accountType = CarrierAccountType.Exchange;
                            break;

                        case SourceAccountType.Termination:
                            accountType = CarrierAccountType.Supplier;
                            break;

                        default:
                            accountType = CarrierAccountType.Exchange;
                            break;
                    }


                    carrierAccountSettings.Mask = sourceItem.CarrierMask;
                    return new CarrierAccount
                    {
                        AccountType = accountType,
                        CarrierAccountSettings = carrierAccountSettings,
                        CarrierProfileId = carrierProfile.CarrierProfileId,
                        CustomerSettings = carrierAccountCustomerSettings,
                        NameSuffix = (String.IsNullOrEmpty(sourceItem.NameSuffix) ? carrierProfile.SourceId : sourceItem.NameSuffix),
                        //SellingNumberPlanId = null,
                        SupplierSettings = carrierAccountSupplierSettings,
                        SourceId = sourceItem.SourceId
                    };
                }

                else
                    return null;

            }
            else
                return null;
        }
    }
}
