using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CarrierAccountMigrator : Migrator<SourceCarrierAccount, CarrierAccount>
    {
        CarrierAccountDBSyncDataManager dbSyncDataManager;
        SourceCarrierAccountDataManager dataManager;
        DBTable dbTableCurrency;
        DBTable dbTableCarrierProfile;

        public CarrierAccountMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierAccountDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCarrierAccountDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            dbTableCurrency = Context.DBTables[DBTableName.Currency];
            dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CarrierAccount> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierAccountsToTemp(itemsToAdd);
            DBTable dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            if (dbTableCarrierAccount != null)
                dbTableCarrierAccount.Records = dbSyncDataManager.GetCarrierAccounts();
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

            if (dbTableCarrierProfile != null && dbTableCurrency != null)
            {
                var allCarrierProfiles = (Dictionary<string, CarrierProfile>)dbTableCarrierProfile.Records;
                CarrierProfile carrierProfile = null;

                if (allCarrierProfiles != null)
                    allCarrierProfiles.TryGetValue(sourceItem.ProfileId.ToString(), out carrierProfile);

                var allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId.ToString(), out currency);

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
                    int? sellingNumberPlanId = null;
                    if (accountType != CarrierAccountType.Supplier)
                        sellingNumberPlanId = Context.DefaultSellingNumberPlanId;

                    return new CarrierAccount
                    {
                        AccountType = accountType,
                        CarrierAccountSettings = carrierAccountSettings,
                        CarrierProfileId = carrierProfile.CarrierProfileId,
                        CustomerSettings = carrierAccountCustomerSettings,
                        NameSuffix = (String.IsNullOrEmpty(sourceItem.NameSuffix) ? carrierProfile.SourceId : sourceItem.NameSuffix),
                        SellingNumberPlanId = sellingNumberPlanId,
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
