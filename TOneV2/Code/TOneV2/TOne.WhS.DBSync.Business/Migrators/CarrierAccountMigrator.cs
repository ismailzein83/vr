using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using System.Data.SqlClient;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class CarrierAccountMigrator : Migrator<SourceCarrierAccount, CarrierAccount>
    {
        CarrierAccountDBSyncDataManager dbSyncDataManager;
        SourceCarrierAccountDataManager dataManager;
        Dictionary<string, CarrierProfile> allCarrierProfiles;
        Dictionary<string, Currency> allCurrencies;
        Dictionary<string, ZoneServiceConfig> allZoneServicesConfig;
        Dictionary<string, VRTimeZone> allTimeZones;
        MigrationContext _context;
        public CarrierAccountMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierAccountDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCarrierAccountDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            var dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            allCarrierProfiles = (Dictionary<string, CarrierProfile>)dbTableCarrierProfile.Records;
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            var dbTableZoneServicesConfig = Context.DBTables[DBTableName.ZoneServiceConfig];
            allZoneServicesConfig = (Dictionary<string, ZoneServiceConfig>)dbTableZoneServicesConfig.Records;
            var dbTableVRTimeZone = context.DBTables[DBTableName.VRTimeZone];
            allTimeZones = (Dictionary<String, VRTimeZone>)dbTableVRTimeZone.Records;
            _context = context;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
            FinalizeRelatedEntities();
        }

        public override void AddItems(List<CarrierAccount> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierAccountsToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;

        }

        public override IEnumerable<SourceCarrierAccount> GetSourceItems()
        {
            return dataManager.GetSourceCarrierAccounts().OrderBy(itm => itm.SourceId);
        }

        public override CarrierAccount BuildItemFromSource(SourceCarrierAccount sourceItem)
        {
            CarrierAccountSettings carrierAccountSettings = new CarrierAccountSettings();
            CarrierAccountCustomerSettings carrierAccountCustomerSettings = new CarrierAccountCustomerSettings { PricelistSettings = new PricelistSettings() };
            CarrierAccountSupplierSettings carrierAccountSupplierSettings = new CarrierAccountSupplierSettings();

            CarrierProfile carrierProfile = null;

            if (allCarrierProfiles != null)
                allCarrierProfiles.TryGetValue(sourceItem.ProfileId.ToString(), out carrierProfile);


            Currency currency = null;
            if (allCurrencies != null && sourceItem.CurrencyId != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId.ToString(), out currency);

            if (carrierProfile != null)
            {
                if (currency != null)
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

                switch (sourceItem.RoutingStatus)
                {
                    case SourceRoutingStatus.Blocked:
                        carrierAccountCustomerSettings.RoutingStatus = RoutingStatus.Blocked;
                        carrierAccountSupplierSettings.RoutingStatus = RoutingStatus.Blocked;
                        break;
                    case SourceRoutingStatus.BlockedInbound:
                        carrierAccountCustomerSettings.RoutingStatus = RoutingStatus.Blocked;
                        carrierAccountSupplierSettings.RoutingStatus = RoutingStatus.Enabled;
                        break;
                    case SourceRoutingStatus.BlockedOutbound:
                        carrierAccountCustomerSettings.RoutingStatus = RoutingStatus.Enabled;
                        carrierAccountSupplierSettings.RoutingStatus = RoutingStatus.Blocked;
                        break;
                    case SourceRoutingStatus.Enabled:
                        carrierAccountCustomerSettings.RoutingStatus = RoutingStatus.Enabled;
                        carrierAccountSupplierSettings.RoutingStatus = RoutingStatus.Enabled;
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

                FillTimeZoneAndDefaultServicesForCarrierAccount(sourceItem, carrierAccountCustomerSettings, carrierAccountSupplierSettings);

                if (sourceItem.AccountType == SourceAccountType.Client || sourceItem.AccountType == SourceAccountType.Exchange)
                {
                    if (sourceItem.IsAToZ)
                    {
                        carrierAccountCustomerSettings.PricelistSettings.PriceListType = SalePriceListType.Full;
                    }
                }

                if (sourceItem.NominalCapacityInE1s.HasValue)
                    carrierAccountSettings.NominalCapacity = sourceItem.NominalCapacityInE1s.Value;
                carrierAccountSettings.IsInterconnectSwitch = sourceItem.RepresentsASwitch;
                return new CarrierAccount
                {
                    AccountType = accountType,
                    CarrierAccountSettings = carrierAccountSettings,
                    CarrierProfileId = carrierProfile.CarrierProfileId,
                    CustomerSettings = carrierAccountCustomerSettings,
                    NameSuffix = sourceItem.NameSuffix,
                    SellingNumberPlanId = accountType == CarrierAccountType.Supplier ? null : (int?)Context.DefaultSellingNumberPlanId,
                    SupplierSettings = carrierAccountSupplierSettings,
                    SourceId = sourceItem.SourceId,
                    IsDeleted = sourceItem.IsDeleted,
                    SellingProductId = accountType == CarrierAccountType.Supplier ? null : (int?)Context.SellingProductId
                };
            }

            else
            {
                TotalRowsFailed++;
                Context.WriteWarning(string.Format("Failed migrating Carrier Account {0}, Source Id: {1}", sourceItem.NameSuffix, sourceItem.SourceId));
                return null;
            }


        }

        public override void FinalizeRelatedEntities()
        {
            if (_context.MigrationRequestedTables.Contains(DBTableName.CarrierAccount))
            {
                ClearTempTablesDBSyncDataManager clearTempTablesDBSyncDataManager = new ClearTempTablesDBSyncDataManager(_context.UseTempTables);
                clearTempTablesDBSyncDataManager.ClearTempTables();
            }
        }

        private void FillTimeZoneAndDefaultServicesForCarrierAccount(SourceCarrierAccount sourceItem, CarrierAccountCustomerSettings carrierAccountCustomerSettings, CarrierAccountSupplierSettings carrierAccountSupplierSettings)
        {
            if (sourceItem.AccountType == SourceAccountType.Client && sourceItem.CustomerGMTTime.HasValue)
                FillTimeZoneForCustomer(sourceItem.CustomerGMTTime.Value, carrierAccountCustomerSettings);
            else if (sourceItem.AccountType == SourceAccountType.Termination && sourceItem.GMTTime.HasValue)
            {
                FillTimeZoneForSupplier(sourceItem.GMTTime.Value, carrierAccountSupplierSettings);
                FillDefualtServices(sourceItem, carrierAccountSupplierSettings);
            }
            else
            {
                FillTimeZoneForCustomer(sourceItem.CustomerGMTTime.Value, carrierAccountCustomerSettings);
                FillTimeZoneForSupplier(sourceItem.GMTTime.Value, carrierAccountSupplierSettings);
                FillDefualtServices(sourceItem, carrierAccountSupplierSettings);
            }
        }

        private void FillTimeZoneForSupplier(short supplierOffset, CarrierAccountSupplierSettings carrierAccountSupplierSettings)
        {
            TimeSpan offSet = TimeSpan.FromMinutes(supplierOffset);
            VRTimeZone timeZone = allTimeZones.Values.FindRecord(item => item.Settings != null && item.Settings.Offset == offSet);

            if (timeZone != null)
                carrierAccountSupplierSettings.TimeZoneId = timeZone.TimeZoneId;
        }

        private void FillTimeZoneForCustomer(short customerOffset, CarrierAccountCustomerSettings carrierAccountCustomerSettings)
        {
            TimeSpan offSet = TimeSpan.FromMinutes(customerOffset);
            VRTimeZone timeZone = allTimeZones.Values.FindRecord(item => item.Settings != null && item.Settings.Offset == offSet);

            if (timeZone != null)
                carrierAccountCustomerSettings.TimeZoneId = timeZone.TimeZoneId;
        }


        private void FillDefualtServices(SourceCarrierAccount sourceItem, CarrierAccountSupplierSettings carrierAccountSupplierSettings)
        {
            List<ZoneService> defaultServices = new List<ZoneService>();
            if (allZoneServicesConfig != null)
            {
                foreach (KeyValuePair<string, ZoneServiceConfig> item in allZoneServicesConfig)
                {
                    if ((Convert.ToInt32(item.Value.SourceId) & Convert.ToInt32(sourceItem.ServicesFlag)) == Convert.ToInt32(item.Value.SourceId))
                        defaultServices.Add(new ZoneService()
                        {
                            ServiceId = Convert.ToInt32(item.Value.ZoneServiceConfigId)
                        });
                }
            }
            carrierAccountSupplierSettings.DefaultServices = defaultServices;
        }
        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            if (dbTableCarrierAccount != null)
                dbTableCarrierAccount.Records = dbSyncDataManager.GetCarrierAccounts(useTempTables);
        }
    }
}
