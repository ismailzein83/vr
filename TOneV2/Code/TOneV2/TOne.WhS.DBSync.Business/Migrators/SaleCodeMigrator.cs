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
        MigrationContext _context;
      
        
        public SaleCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
           _context = context;
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
            TotalRowsSuccess = itemsToAdd.Count;
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
            CodeGroupMigrator codeGroupMigrator = new CodeGroupMigrator(_context);
            CodeGroup codeGroup = codeGroupMigrator.GetMatchCodeGroup(sourceItem.Code);
           
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
            {
                TotalRowsFailed++;
                return null;
            }
        }

        public override void FillTableInfo(bool useTempTables)
        {

        }

       
    }
}
