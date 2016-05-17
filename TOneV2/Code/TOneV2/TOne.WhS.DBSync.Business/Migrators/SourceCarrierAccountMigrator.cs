using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using System;
using TOne.WhS.DBSync.Data.SQL.Common;

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
            DBTable dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            if (dbTableCarrierProfile != null)
            {
                List<CarrierProfile> allCarrierProfiles = (List<CarrierProfile>)dbTableCarrierProfile.Records;
                CarrierProfile carrierProfile = allCarrierProfiles.Where(x => x.SourceId == sourceItem.ProfileId.ToString()).FirstOrDefault();
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
