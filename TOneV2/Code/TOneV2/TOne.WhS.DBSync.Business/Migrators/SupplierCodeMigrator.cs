using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierCodeMigrator : Migrator<SourceCode, SupplierCode>
    {
        SupplierCodeDBSyncDataManager dbSyncDataManager;
        SourceCodeDataManager dataManager;

        public SupplierCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SupplierCode> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySupplierCodesToTemp(itemsToAdd, startingId);
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(false);
        }

        public override SupplierCode BuildItemFromSource(SourceCode sourceItem)
        {
            DBTable dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];

            if (dbTableSupplierZone != null && dbTableCodeGroup != null)
            {
                Dictionary<string, SupplierZone> allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
                Dictionary<string, CodeGroup> allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;

                SupplierZone supplierZone = null;
                if (allSupplierZones != null)
                    allSupplierZones.TryGetValue(sourceItem.ZoneId.ToString(), out supplierZone);

                CodeGroup codeGroup = null;
                if (allCodeGroups != null)
                    allCodeGroups.TryGetValue(sourceItem.CodeGroup, out codeGroup);

                if (codeGroup != null & supplierZone != null)
                    return new SupplierCode
                    {
                        Code = sourceItem.Code,
                        BED = sourceItem.BeginEffectiveDate,
                        CodeGroupId = codeGroup.CodeGroupId,
                        EED = sourceItem.EndEffectiveDate,
                        ZoneId = supplierZone.SupplierZoneId,
                        SourceId = sourceItem.SourceId
                    };
                else
                    return null;

            }
            else
                return null;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(SupplierCodeManager), nbOfIds, out startingId);
        }
    }
}
