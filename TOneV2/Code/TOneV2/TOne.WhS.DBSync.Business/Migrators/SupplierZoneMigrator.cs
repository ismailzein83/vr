using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierZoneMigrator : Migrator<SourceZone, SupplierZone>
    {
        SupplierZoneDBSyncDataManager dbSyncDataManager;
        SourceZoneDataManager dataManager;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        Dictionary<string, CodeGroup> allCodeGroups;
        bool _onlyEffective;

        public SupplierZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierZoneDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            var dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
            allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;
            _onlyEffective = context.OnlyEffective;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSupplierZoneTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SupplierZone> itemsToAdd)
        {
            dbSyncDataManager.ApplySupplierZonesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceZone> GetSourceItems()
        {
            return dataManager.GetSourceZones(false, _onlyEffective);
        }

        public override SupplierZone BuildItemFromSource(SourceZone sourceItem)
        {
            CarrierAccount carrierAccount = null;
            if (allCarrierAccounts != null)
                allCarrierAccounts.TryGetValue(sourceItem.SupplierId, out carrierAccount);

            CodeGroup codeGroup = null;
            if (allCodeGroups != null)
                allCodeGroups.TryGetValue(sourceItem.CodeGroup, out codeGroup);

            if (codeGroup != null && carrierAccount != null)
                return new SupplierZone
                {
                    SupplierId = carrierAccount.CarrierAccountId,
                    BED = sourceItem.BED,
                    CountryId = codeGroup.CountryId,
                    EED = sourceItem.EED,
                    Name = sourceItem.Name,
                    SourceId = sourceItem.SourceId
                };
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            if (dbTableSupplierZone != null)
                dbTableSupplierZone.Records = dbSyncDataManager.GetSupplierZones(useTempTables);
        }
    }
}
