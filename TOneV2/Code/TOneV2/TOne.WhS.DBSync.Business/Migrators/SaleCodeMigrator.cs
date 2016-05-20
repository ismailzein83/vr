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
    public class SaleCodeMigrator : Migrator<SourceCode, SaleCode>
    {
        SaleCodeDBSyncDataManager dbSyncDataManager;
        SourceCodeDataManager dataManager;

        public SaleCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SaleCode> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySaleCodesToTemp(itemsToAdd, startingId);
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(true);
        }

        public override SaleCode BuildItemFromSource(SourceCode sourceItem)
        {
            DBTable dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];

            if (dbTableSaleZone != null && dbTableCodeGroup != null)
            {
                Dictionary<string, SaleZone> allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
                Dictionary<string, CodeGroup> allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;

                SaleZone saleZone = null;
                if (allSaleZones != null)
                    allSaleZones.TryGetValue(sourceItem.ZoneId.ToString(), out saleZone);

                CodeGroup codeGroup = null;
                if (allCodeGroups != null)
                    allCodeGroups.TryGetValue(sourceItem.CodeGroup, out codeGroup);

                if (codeGroup != null & saleZone != null)
                    return new SaleCode
                        {
                            Code = sourceItem.Code,
                            BED = sourceItem.BeginEffectiveDate,
                            CodeGroupId = codeGroup.CodeGroupId,
                            EED = sourceItem.EndEffectiveDate,
                            ZoneId = saleZone.SaleZoneId,
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
            IDManager.Instance.ReserveIDRange(typeof(SaleCodeManager), nbOfIds, out startingId);
        }
    }
}
