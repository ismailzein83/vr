﻿using System.Collections.Generic;
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
        Dictionary<string, CodeGroup> allCodeGroups;

        public SupplierCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            var dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
            allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;
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
            dbSyncDataManager.ApplySupplierCodesToTemp(itemsToAdd, 1);
            TotalRows = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(false);
        }

        public override SupplierCode BuildItemFromSource(SourceCode sourceItem)
        {


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

        public override void FillTableInfo(bool useTempTables)
        {

        }
    }

   
}
