using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.DBSync.Business
{
    public class SaleCodeMigrator : Migrator<SourceCode, SaleCode>
    {
        SaleCodeDBSyncDataManager dbSyncDataManager;
        SourceCodeDataManager dataManager;
        Dictionary<string, SaleZone> allSaleZones;
        Dictionary<string, CodeGroup> allCodeGroups;
        public SaleCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            var dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
            allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SaleCodeManager manager = new SaleCodeManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSaleCodeTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SaleCode> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleCodesToTemp(itemsToAdd, 1);
            TotalRows = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(true);
        }

        public override SaleCode BuildItemFromSource(SourceCode sourceItem)
        {
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

        public override void FillTableInfo(bool useTempTables)
        {
           
        }
    }
}
