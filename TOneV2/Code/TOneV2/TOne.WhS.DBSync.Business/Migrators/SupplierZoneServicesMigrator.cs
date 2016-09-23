using System;
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
    public class SupplierZoneServicesMigrator : Migrator<SourceRate, SupplierZoneService>
    {
        SupplierZoneServicesDBSyncDataManager dbSyncDataManager;
        SourceZoneServiceDataManager dataManager;
        Dictionary<string, SupplierZone> allSupplierZones;
        Dictionary<string, ZoneServiceConfig> allZoneServicesConfig;
        public SupplierZoneServicesMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierZoneServicesDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneServiceDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
            var dbTableZoneServicesConfig = Context.DBTables[DBTableName.ZoneServiceConfig];
            allZoneServicesConfig = (Dictionary<string, ZoneServiceConfig>)dbTableZoneServicesConfig.Records;

        }

        public override void Migrate(MigrationInfoContext context)
        {
            SupplierZoneServiceManager manager = new SupplierZoneServiceManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSupplierZoneServiceTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SupplierZoneService> itemsToAdd)
        {
            dbSyncDataManager.ApplySupplierZoneServicesToTemp(itemsToAdd, TotalRowsSuccess + 1);
            TotalRowsSuccess = TotalRowsSuccess + itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceZoneServices(false);
        }

        public override SupplierZoneService BuildItemFromSource(SourceRate sourceItem)
        {
            SupplierZone supplierZone = null;


            if (allSupplierZones != null && sourceItem.ZoneId.HasValue)
                allSupplierZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out supplierZone);

            if (supplierZone != null && sourceItem.BeginEffectiveDate.HasValue)
            {
                List<ZoneService> effectiveServices = new List<ZoneService>();

                if (allZoneServicesConfig != null)
                {
                    foreach (KeyValuePair<string, ZoneServiceConfig> item in allZoneServicesConfig)
                    {
                        if ((Convert.ToInt32(item.Value.SourceId) & Convert.ToInt32(sourceItem.ServicesFlag.Value)) == Convert.ToInt32(item.Value.SourceId))
                            effectiveServices.Add(new ZoneService()
                            {
                                ServiceId = Convert.ToInt32(item.Value.ZoneServiceConfigId)
                            });
                    }
                }


                return new SupplierZoneService
                {
                    BED = sourceItem.BeginEffectiveDate.Value,
                    EED = sourceItem.EndEffectiveDate,
                    EffectiveServices = effectiveServices,
                    ReceivedServices = effectiveServices,
                    ZoneId = supplierZone.SupplierZoneId,
                    SourceId = sourceItem.SourceId
                };
            }
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }


        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override void LoadSourceItems(Action<SourceRate> onItemLoaded)
        {
            dataManager.LoadSourceItems(false, onItemLoaded);
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
