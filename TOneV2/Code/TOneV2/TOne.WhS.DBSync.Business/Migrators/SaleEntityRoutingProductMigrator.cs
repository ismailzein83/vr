using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class SaleEntityRoutingProductMigrator : Migrator<SourceRate, SaleEntityRoutingProduct>
    {

        SourceRateDataManager dataManager;
        SaleEntityRoutingProductDBSyncDataManager dbSyncDataManager;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        Dictionary<string, ZoneServiceConfig> allZoneServicesConfig;
        Dictionary<string, string> _MatchedServicesBySourceId;
        Dictionary<string, RoutingProduct> allRoutingProductsByServiceIds;
        Dictionary<string, SaleZone> allSaleZones;
        bool _onlyEffective;
        public SaleEntityRoutingProductMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleEntityRoutingProductDBSyncDataManager(context.UseTempTables);

            var dbTableCarrierAccounts = Context.DBTables[DBTableName.CarrierAccount];
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccounts.Records;

            var dbTableZoneServicesConfig = Context.DBTables[DBTableName.ZoneServiceConfig];
            allZoneServicesConfig = (Dictionary<string, ZoneServiceConfig>)dbTableZoneServicesConfig.Records;

            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;

            dataManager = new SourceRateDataManager(Context.ConnectionString, Context.EffectiveAfterDate, Context.OnlyEffective);

            _MatchedServicesBySourceId = new Dictionary<string, string>();
            StructureRoutingProductsByServiceIds();

            _onlyEffective = Context.OnlyEffective;

            TableName = dbSyncDataManager.GetTableName();

        }

        void StructureRoutingProductsByServiceIds()
        {
            allRoutingProductsByServiceIds = new Dictionary<string, RoutingProduct>();
            RoutingProductManager routingProductManager = new RoutingProductManager();
            Dictionary<int, RoutingProduct> routingProducts = routingProductManager.GetAllRoutingProducts();
            foreach (var routingProduct in routingProducts.Values)
            {
                allRoutingProductsByServiceIds.Add(string.Join(",", routingProduct.Settings.DefaultServiceIds.OrderBy(s => s)), routingProduct);
            }

        }
        public override void FillTableInfo(bool useTempTables)
        {

        }
        public override void Migrate(MigrationInfoContext context)
        {
            SaleEntityRoutingProductManager manager = new SaleEntityRoutingProductManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSaleEntityRoutingProductTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SaleEntityRoutingProduct> itemsToAdd)
        {

            dbSyncDataManager.ApplySaleEntityRoutingProductsToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(true);
        }

        public override SaleEntityRoutingProduct BuildItemFromSource(SourceRate sourceItem)
        {
            throw new NotImplementedException();
        }

        public override List<SaleEntityRoutingProduct> BuildAllItemsFromSource(IEnumerable<SourceRate> sourceItems)
        {
            List<SaleEntityRoutingProduct> result = new List<SaleEntityRoutingProduct>();
            result.Add(new SaleEntityRoutingProduct
            {
                BED = new DateTime(2000, 1, 1),
                EED = null,
                OwnerId = 1,
                OwnerType = SalePriceListOwnerType.SellingProduct,
                RoutingProductId = 1,
                SaleZoneId = null
            });

            Dictionary<int, Dictionary<string, SaleEntityRateByCustomer>> data = new Dictionary<int, Dictionary<string, SaleEntityRateByCustomer>>();
            Dictionary<int, SaleEntityRateByCustomerWithMaxCount> saleEntityRateByCustomerMaxCount = new Dictionary<int, SaleEntityRateByCustomerWithMaxCount>();
            foreach (var sourceRate in sourceItems)
            {
                CarrierAccount carrierAccount;
                if (allCarrierAccounts.TryGetValue(sourceRate.CustomerId, out carrierAccount))
                {
                    SaleEntityRateByCustomerWithMaxCount saleEntityRateByCustomerWithMaxCount = saleEntityRateByCustomerMaxCount.GetOrCreateItem(carrierAccount.CarrierAccountId);

                    Dictionary<string, SaleEntityRateByCustomer> saleEntities = data.GetOrCreateItem(carrierAccount.CarrierAccountId);

                    var saleEntityRateByCustomer = saleEntities.GetOrCreateItem(sourceRate.ServicesFlag.Value.ToString(), () => new SaleEntityRateByCustomer
                    {
                        CustomerId = carrierAccount.CarrierAccountId
                    });
                    saleEntityRateByCustomer.Counter++;
                    if (saleEntityRateByCustomerWithMaxCount.MaxCount < saleEntityRateByCustomer.Counter)
                    {
                        saleEntityRateByCustomerWithMaxCount.MaxCount = saleEntityRateByCustomer.Counter;
                        saleEntityRateByCustomerWithMaxCount.SaleEntityRateByCustomer = saleEntityRateByCustomer;
                    }
                    saleEntityRateByCustomer.Rates.Add(sourceRate);

                }
                else
                {
                    Context.WriteWarning(string.Format("Failed migrating Sale Entity Routing Product, Customer is not found: {0}", sourceRate.CustomerId));
                    this.TotalRowsFailed++;
                }
            }

            foreach (var item in data)
            {
                int customerId = item.Key;
                Dictionary<string, SaleEntityRateByCustomer> saleEntityRateByCustomerByServiceId = item.Value;
                SaleEntityRateByCustomerWithMaxCount maxSaleEntityRate;
                if (!saleEntityRateByCustomerMaxCount.TryGetValue(customerId, out maxSaleEntityRate))
                {
                    Context.WriteWarning(string.Format("Failed migrating Sale Entity Routing Product, Customer ID {0}", customerId));
                    this.TotalRowsFailed++;
                    continue;
                }

                foreach (var saleEntityRateByCustomer in saleEntityRateByCustomerByServiceId)
                {
                    RoutingProduct rp = GetRoutingProduct(saleEntityRateByCustomer.Key);
                    if (rp == null)
                    {
                        Context.WriteWarning(string.Format("Failed migrating Sale Entity Routing Product, Routing Product is not found:Service Id {0}", saleEntityRateByCustomer.Key));
                        this.TotalRowsFailed++;
                        continue;
                    }

                    var saleEntityRate = saleEntityRateByCustomer.Value;
                    if (saleEntityRate == maxSaleEntityRate.SaleEntityRateByCustomer)
                        result.Add(GetDefaultSaleEntityRoutingProduct(maxSaleEntityRate.SaleEntityRateByCustomer, rp));
                    else
                        result.AddRange(GenerateSaleEntityRoutingProducts(saleEntityRate, rp));
                }
            }

            return result;
        }

        SaleEntityRoutingProduct GetDefaultSaleEntityRoutingProduct(SaleEntityRateByCustomer saleEntityRateByCustomer, RoutingProduct rp)
        {
            return new SaleEntityRoutingProduct
            {
                BED = saleEntityRateByCustomer.Rates.Where(r => r.BeginEffectiveDate.HasValue).Min(r => r.BeginEffectiveDate.Value),
                SaleZoneId = null,
                OwnerType = SalePriceListOwnerType.Customer,
                OwnerId = saleEntityRateByCustomer.CustomerId,
                RoutingProductId = rp.RoutingProductId,
                EED = null
            };
        }

        RoutingProduct GetRoutingProduct(string sourceServiceId)
        {
            RoutingProduct rp;
            string serviceIds = GetMatchedServiceIds(sourceServiceId);
            allRoutingProductsByServiceIds.TryGetValue(serviceIds, out rp);
            return rp;
        }

        List<SaleEntityRoutingProduct> GenerateSaleEntityRoutingProducts(SaleEntityRateByCustomer saleEntityRate, RoutingProduct rp)
        {
            List<SaleEntityRoutingProduct> result = new List<SaleEntityRoutingProduct>();
            foreach (var saleRate in saleEntityRate.Rates)
            {
                SaleZone saleZone = null;
                if (allSaleZones != null && saleRate.ZoneId.HasValue)
                    allSaleZones.TryGetValue(saleRate.ZoneId.Value.ToString(), out saleZone);

                if (saleZone != null && saleRate.BeginEffectiveDate.HasValue && saleRate.Rate.HasValue)
                {
                    result.Add(new SaleEntityRoutingProduct
                    {
                        SaleZoneId = saleZone.SaleZoneId,
                        BED = saleRate.BeginEffectiveDate.Value,
                        EED = saleRate.EndEffectiveDate,
                        RoutingProductId = rp.RoutingProductId,
                        OwnerId = saleEntityRate.CustomerId,
                        OwnerType = SalePriceListOwnerType.Customer
                    });
                }
                else
                {
                    //Context.WriteWarning(string.Format("Failed migrating Sale Entity Routing Product, Sale Zone is not found: {0}", saleRate.ZoneId));
                    this.TotalRowsFailed++;
                }
            }

            return result;

        }

        string GetMatchedServiceIds(string sourceId)
        {
            string concatServiceIds;
            if (!_MatchedServicesBySourceId.TryGetValue(sourceId, out concatServiceIds))
            {
                HashSet<int> serviceIds = new HashSet<int>();
                ZoneServiceConfig zoneServiceConfig;

                if (allZoneServicesConfig.TryGetValue(sourceId, out zoneServiceConfig))
                {
                    serviceIds.Add(zoneServiceConfig.ZoneServiceConfigId);
                }
                else
                {
                    foreach (KeyValuePair<string, ZoneServiceConfig> item in allZoneServicesConfig)
                    {
                        int serviceId = Convert.ToInt32(item.Value.SourceId);
                        if ((serviceId & Convert.ToInt32(sourceId)) == serviceId)
                            serviceIds.Add(item.Value.ZoneServiceConfigId);
                    }
                }
                concatServiceIds = string.Join(",", serviceIds.OrderBy(s => s));
            }
            return concatServiceIds;
        }

        public override bool IsBuildAllItemsOnce
        {
            get
            {
                return true;
            }
        }
    }
    class SaleEntityRateByCustomer
    {
        public SaleEntityRateByCustomer()
        {
            Rates = new List<SourceRate>();
        }
        public int CustomerId { get; set; }
        public int Counter { get; set; }
        public List<SourceRate> Rates { get; set; }
    }

    class SaleEntityRateByCustomerWithMaxCount
    {
        public SaleEntityRateByCustomer SaleEntityRateByCustomer { get; set; }
        public int MaxCount { get; set; }
    }

}
