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
    public class SupplierCodeMigrator : Migrator<SourceCode, SupplierCode>
    {
        SupplierCodeDBSyncDataManager dbSyncDataManager;
        SourceCodeDataManager dataManager;
        Dictionary<string, SupplierZone> allSupplierZones;
        TOne.WhS.BusinessEntity.Business.CodeIterator<CodeGroup> _codeGroupIterator;
        bool _onlyEffective;

        public SupplierCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
            var codeGroups = (Dictionary<string, CodeGroup>)context.DBTables[DBTableName.CodeGroup].Records;
            _codeGroupIterator = new CodeIterator<CodeGroup>(codeGroups.Values);
            _onlyEffective = context.OnlyEffective;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SupplierCodeManager manager = new SupplierCodeManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSupplierCodeTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SupplierCode> itemsToAdd)
        {
            dbSyncDataManager.ApplySupplierCodesToTemp(itemsToAdd, TotalRowsSuccess + 1);
            TotalRowsSuccess = TotalRowsSuccess + itemsToAdd.Count;
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(false, _onlyEffective);
        }

        public override SupplierCode BuildItemFromSource(SourceCode sourceItem)
        {


            SupplierZone supplierZone = null;
            if (allSupplierZones != null)
                allSupplierZones.TryGetValue(sourceItem.ZoneId.ToString(), out supplierZone);

            CodeGroup codeGroup = _codeGroupIterator.GetLongestMatch(sourceItem.Code);

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
            dataManager.LoadSourceItems(false,onItemLoaded);
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
