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
    public class SaleZoneServicesMigrator : Migrator<SourceRate, SaleEntityZoneService>
    {
        SaleZoneServicesDBSyncDataManager dbSyncDataManager;
        SourceZoneServiceDataManager dataManager;
        Dictionary<string, SaleZone> allSaleZones;
        Dictionary<string, ZoneServiceConfig> allZoneServicesConfig;
        Dictionary<string, SalePriceList> allSalePriceLists;
        bool _onlyEffective;
        public SaleZoneServicesMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleZoneServicesDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneServiceDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();

            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;

            var dbTableZoneServicesConfig = Context.DBTables[DBTableName.ZoneServiceConfig];
            allZoneServicesConfig = (Dictionary<string, ZoneServiceConfig>)dbTableZoneServicesConfig.Records;
            
            var dbTableSalePriceList = Context.DBTables[DBTableName.SalePriceList];
            allSalePriceLists = (Dictionary<string, SalePriceList>)dbTableSalePriceList.Records;

            _onlyEffective = context.OnlyEffective;

        }

        public override void Migrate(MigrationInfoContext context)
        {
            SaleEntityServiceManager manager = new SaleEntityServiceManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSaleEntityServiceTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SaleEntityZoneService> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleZoneServicesToTemp(itemsToAdd, TotalRowsSuccess + 1);
            TotalRowsSuccess = TotalRowsSuccess + itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceZoneServices(true, _onlyEffective);
        }

        public override SaleEntityZoneService BuildItemFromSource(SourceRate sourceItem)
        {
            SaleZone saleZone = null;
            if (allSaleZones != null && sourceItem.ZoneId.HasValue)
                allSaleZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out saleZone);

            SalePriceList salePriceList = null;
            if (allSalePriceLists != null && sourceItem.PriceListId.HasValue)
                allSalePriceLists.TryGetValue(sourceItem.PriceListId.Value.ToString(), out salePriceList);

            if (saleZone != null && salePriceList != null && sourceItem.BeginEffectiveDate.HasValue)
            {
                List<ZoneService> effectiveServices = new List<ZoneService>();

                foreach (KeyValuePair<string, ZoneServiceConfig> item in allZoneServicesConfig)
                {
                    if ((Convert.ToInt32(item.Value.SourceId) & Convert.ToInt32(sourceItem.ServicesFlag.Value)) == Convert.ToInt32(item.Value.SourceId))
                        effectiveServices.Add(new ZoneService()
                        {
                            ServiceId = Convert.ToInt32(item.Value.ZoneServiceConfigId)
                        });
                }

                return new SaleEntityZoneService
                {
                    BED = sourceItem.BeginEffectiveDate.Value,
                    EED = sourceItem.EndEffectiveDate,
                    Services = effectiveServices,
                    PriceListId = salePriceList.PriceListId,
                    ZoneId = saleZone.SaleZoneId,
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
