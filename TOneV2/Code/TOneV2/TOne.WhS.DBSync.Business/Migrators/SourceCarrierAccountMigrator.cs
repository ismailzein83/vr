using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierAccountMigrator
    {
        private string _ConnectionString;
        private bool _UseTempTables;
        private DBSyncLogger _Logger;
        public SourceCarrierAccountMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
            _ConnectionString = connectionString;
        }

        public void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'CarrierAccount' started");

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

            _Logger.WriteInformation("Migrating table 'CarrierAccount' ended");
        }

        private void AddItems(List<CarrierAccount> itemsToAdd, List<DBTable> context)
        {
            CarrierAccountDBSyncManager CarrierAccountManager = new CarrierAccountDBSyncManager(_UseTempTables);
            CarrierAccountManager.ApplyCarrierAccountsToTemp(itemsToAdd);
            DBTable dbTableCarrierAccount = context.Where(x => x.Name == Constants.Table_CarrierAccount).FirstOrDefault();
            if (dbTableCarrierAccount != null)
                dbTableCarrierAccount.Records = CarrierAccountManager.GetCarrierAccounts();
        }

        private IEnumerable<SourceCarrierAccount> GetSourceItems()
        {
            SourceCarrierAccountDataManager dataManager = new SourceCarrierAccountDataManager(_ConnectionString);
            return dataManager.GetSourceCarrierAccounts();
        }

        private CarrierAccount BuildItemFromSource(SourceCarrierAccount sourceItem, List<DBTable> context)
        {
            return new CarrierAccount
            {
                AccountType = CarrierAccountType.Customer,/// Change to real
                CarrierAccountSettings = new CarrierAccountSettings(),
                CarrierProfileId = 0,
                CustomerSettings = new CarrierAccountCustomerSettings(),
                NameSuffix = "",
                SellingNumberPlanId = null,
                SupplierSettings = new CarrierAccountSupplierSettings()
            };
        }
    }
}
