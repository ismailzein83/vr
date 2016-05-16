using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using System;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierAccountMigrator : Migrator
    {
        public SourceCarrierAccountMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
            : base(connectionString, useTempTables, logger)
        {
        }

        public override void Migrate(List<DBTable> context)
        {
            Logger.WriteInformation("Migrating table 'CarrierAccount' started");

            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<CarrierAccount> itemsToAdd = new List<CarrierAccount>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem, context);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }

            Logger.WriteInformation("Migrating table 'CarrierAccount' ended");
        }

        private void AddItems(List<CarrierAccount> itemsToAdd, List<DBTable> context)
        {
            CarrierAccountDBSyncManager CarrierAccountManager = new CarrierAccountDBSyncManager(UseTempTables);
            CarrierAccountManager.ApplyCarrierAccountsToTemp(itemsToAdd);
            DBTable dbTableCarrierAccount = context.Where(x => x.Name == Constants.Table_CarrierAccount).FirstOrDefault();
            if (dbTableCarrierAccount != null)
                dbTableCarrierAccount.Records = CarrierAccountManager.GetCarrierAccounts();
        }

        private IEnumerable<SourceCarrierAccount> GetSourceItems()
        {
            SourceCarrierAccountDataManager dataManager = new SourceCarrierAccountDataManager(ConnectionString);
            return dataManager.GetSourceCarrierAccounts();
        }

        private CarrierAccount BuildItemFromSource(SourceCarrierAccount sourceItem, List<DBTable> context)
        {
            DBTable dbTableCarrierProfile = context.Where(x => x.Name == Constants.Table_CarrierProfile).FirstOrDefault();
            if (dbTableCarrierProfile != null)
            {
                List<CarrierProfile> allCurrencies = (List<CarrierProfile>)dbTableCarrierProfile.Records;
                CarrierProfile carrierProfile = allCurrencies.Where(x => x.SourceId == sourceItem.ProfileId.ToString()).FirstOrDefault();
                if (carrierProfile != null)
                    return new CarrierAccount
            {
                AccountType = CarrierAccountType.Customer,/// Change to real
                CarrierAccountSettings = new CarrierAccountSettings(),
                CarrierProfileId = carrierProfile.CarrierProfileId,
                CustomerSettings = new CarrierAccountCustomerSettings(),
                NameSuffix = (String.IsNullOrEmpty(sourceItem.NameSuffix) ? carrierProfile.SourceId : sourceItem.NameSuffix),
                SellingNumberPlanId = null,
                SupplierSettings = new CarrierAccountSupplierSettings(),
                SourceId = sourceItem.SourceId
            };
                else
                    return null;

            }
            else
                return null;
        }



    }
}
