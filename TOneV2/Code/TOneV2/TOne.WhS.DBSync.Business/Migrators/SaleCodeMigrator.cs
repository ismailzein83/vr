using System;
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
        TOne.WhS.BusinessEntity.Business.CodeIterator<CodeGroup> _codeGroupIterator;
        bool _onlyEffective;
      
        
        public SaleCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
            var codeGroups = (Dictionary<string, CodeGroup>)context.DBTables[DBTableName.CodeGroup].Records;
            _codeGroupIterator = new CodeIterator<CodeGroup>(codeGroups.Values);
            _onlyEffective = context.OnlyEffective;
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
            dbSyncDataManager.ApplySaleCodesToTemp(itemsToAdd, TotalRowsSuccess + 1);
            TotalRowsSuccess = TotalRowsSuccess + itemsToAdd.Count;
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(true, _onlyEffective);
        }

        public override SaleCode BuildItemFromSource(SourceCode sourceItem)
        {
            SaleZone saleZone = null;
            if (allSaleZones != null)
                allSaleZones.TryGetValue(sourceItem.ZoneId.ToString(), out saleZone);

            CodeGroup codeGroup = _codeGroupIterator.GetLongestMatch(sourceItem.Code);
           
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

        public override void LoadSourceItems(Action<SourceCode> onItemLoaded)
        {
            dataManager.LoadSourceItems(true, _onlyEffective, onItemLoaded);
        }

        public override bool IsLoadItemsApproach
        {
            get
            {
                return true;
            }
        }
       
    }
}
