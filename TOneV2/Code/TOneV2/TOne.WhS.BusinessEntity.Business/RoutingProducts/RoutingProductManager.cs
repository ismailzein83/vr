using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RoutingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            var allRoutingProducts = GetAllRoutingProducts();

            Func<RoutingProduct, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRoutingProducts.ToBigResult(input, filterExpression));
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlanId)
        {
            var routingProducts = GetAllRoutingProducts();
            return routingProducts.MapRecords(RoutingProductInfoMapper, x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductInfo(RoutingProductInfoFilter filter)
        {
            var routingProducts = GetAllRoutingProducts();

            HashSet<int> filteredRoutingProductIds = null;
            int? excludedRoutingProductId = null;

            Func<RoutingProduct, bool> filterAssignableToSaleEntity = null;

            Func<RoutingProduct, bool> filterPredicate = (rp) =>
                (
                    (!excludedRoutingProductId.HasValue || rp.RoutingProductId != filter.ExcludedRoutingProductId.Value)
                    &&
                    (filteredRoutingProductIds == null || filteredRoutingProductIds.Contains(rp.RoutingProductId))
                    &&
                    (filterAssignableToSaleEntity == null || filterAssignableToSaleEntity(rp))
                );

            if (filter != null)
            {
                if (filter.AssignableToZoneId.HasValue)
                {
                    filteredRoutingProductIds = GetRoutingProductIdsBySaleZoneId(filter.AssignableToZoneId.Value);
                }
                else if (filter.AssignableToOwnerType.HasValue)
                {
                    if (!filter.AssignableToOwnerId.HasValue)
                        return null;
                    int? sellingNumberPlanId = null;
                    switch (filter.AssignableToOwnerType.Value)
                    {
                        case SalePriceListOwnerType.SellingProduct:
                            SellingProductManager sellingProductManager = new SellingProductManager();
                            sellingNumberPlanId = sellingProductManager.GetSellingNumberPlanId(filter.AssignableToOwnerId.Value);
                            break;
                        case SalePriceListOwnerType.Customer:
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            sellingNumberPlanId = carrierAccountManager.GetCustomerSellingNumberPlanId(filter.AssignableToOwnerId.Value);
                            break;
                    }
                    if (!sellingNumberPlanId.HasValue)
                        return null;
                    else
                    {
                        filterAssignableToSaleEntity = (rp) => (rp.SellingNumberPlanId == sellingNumberPlanId.Value && rp.Settings != null && rp.Settings.ZoneRelationType == RoutingProductZoneRelationType.AllZones);
                    }
                }
                excludedRoutingProductId = filter.ExcludedRoutingProductId;
            }

            

            return routingProducts.MapRecords(RoutingProductInfoMapper, filterPredicate);
        }

        public TOne.Entities.InsertOperationOutput<RoutingProduct> AddRoutingProduct(RoutingProduct routingProduct)
        {
            TOne.Entities.InsertOperationOutput<RoutingProduct> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RoutingProduct>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routingProductId = -1;

            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(routingProduct, out routingProductId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routingProduct.RoutingProductId = routingProductId;
                insertOperationOutput.InsertedObject = routingProduct;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RoutingProduct> UpdateRoutingProduct(RoutingProduct routingProduct)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            bool updateActionSucc = dataManager.Update(routingProduct);
            TOne.Entities.UpdateOperationOutput<RoutingProduct> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RoutingProduct>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = routingProduct;
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRoutingProduct(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            var allRoutingProducts = GetAllRoutingProducts();
            return allRoutingProducts.GetRecord(routingProductId);
        }

        public HashSet<long> GetFilteredZoneIds(int routingProductId)
        {
            string cacheName = String.Format("GetFilteredZoneIds_{0}", routingProductId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   HashSet<long> filteredZoneIds = null;
                   var routingProduct = GetRoutingProduct(routingProductId);
                   if (routingProduct != null && routingProduct.Settings != null)
                   {
                       switch (routingProduct.Settings.ZoneRelationType)
                       {
                           case RoutingProductZoneRelationType.AllZones:
                               break;
                           case RoutingProductZoneRelationType.SpecificZones:
                               if (routingProduct.Settings.Zones == null)
                                   filteredZoneIds = new HashSet<long>();//empty list
                               else
                                   filteredZoneIds = new HashSet<long>(routingProduct.Settings.Zones.Select(zone => zone.ZoneId));
                               break;
                       }
                   }
                   return filteredZoneIds;
               });
        }

        public HashSet<int> GetFilteredSupplierIds(int routingProductId)
        {
            string cacheName = String.Format("GetFilteredSupplierIds_{0}", routingProductId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   HashSet<int> filteredSupplierIds = null;
                   var routingProduct = GetRoutingProduct(routingProductId);
                   if (routingProduct != null && routingProduct.Settings != null)
                   {
                       switch (routingProduct.Settings.SupplierRelationType)
                       {
                           case RoutingProductSupplierRelationType.AllSuppliers:
                               break;
                           case RoutingProductSupplierRelationType.SpecificSuppliers:
                               if (routingProduct.Settings.Suppliers == null)
                                   filteredSupplierIds = new HashSet<int>();//empty list
                               else
                                   filteredSupplierIds = new HashSet<int>(routingProduct.Settings.Suppliers.Select(supplier => supplier.SupplierId));
                               break;
                       }
                   }
                   return filteredSupplierIds;
               });
        }

        public Dictionary<int, RoutingProduct> GetAllRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoutingProducts",
               () =>
               {
                   IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
                   IEnumerable<RoutingProduct> routingProducts = dataManager.GetRoutingProducts();
                   return routingProducts.ToDictionary(kvp => kvp.RoutingProductId, kvp => kvp);
               });
        }

        public HashSet<int> GetRoutingProductIdsBySaleZoneId(long saleZoneId)
        {
            HashSet<int> routingProductIds = new HashSet<int>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneId);
            RoutingProductZoneFinder finder = new RoutingProductZoneFinder();
            Dictionary<int, RoutingProducts> routingProductsByNumberPlan = finder.GetAllRoutingProductsBySellingNumberPlanId();
            RoutingProducts routingProducts;
            if (routingProductsByNumberPlan.TryGetValue(saleZone.SellingNumberPlanId, out routingProducts))
                routingProductIds = routingProducts.GetRoutingProductsByZoneId(saleZoneId);
            return routingProductIds;
        }

        public string GetRoutingProductName(int routingProductId)
        {
            RoutingProduct routingProduct = GetRoutingProduct(routingProductId);
            
            if (routingProductId != null)
                return routingProduct.Name;
            
            return null;
        }

        #region Private Methods

        private RoutingProductInfo RoutingProductInfoMapper(RoutingProduct routingProduct)
        {
            return new RoutingProductInfo()
            {
                RoutingProductId = routingProduct.RoutingProductId,
                Name = routingProduct.Name,
                SellingNumberPlanId = routingProduct.SellingNumberPlanId
            };
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoutingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRoutingProductsUpdated(ref _updateHandle);
            }
        }

        class RoutingProductZoneFinder
        {
            public Dictionary<int, RoutingProducts> GetAllRoutingProductsBySellingNumberPlanId()
            {
                RoutingProductManager manager = new RoutingProductManager();
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoutingProductsBySellingNumberPlan", () =>
                {
                    var routingProducts = manager.GetAllRoutingProducts().Values;
                    Dictionary<int, RoutingProducts> routingProductsByNumberPlan = new Dictionary<int, RoutingProducts>();
                    foreach (RoutingProduct routingProduct in routingProducts)
                    {
                        RoutingProducts currentRoutingProducts;
                        if (!routingProductsByNumberPlan.TryGetValue(routingProduct.SellingNumberPlanId, out currentRoutingProducts))
                        {
                            currentRoutingProducts = new RoutingProducts();
                            routingProductsByNumberPlan.Add(routingProduct.SellingNumberPlanId, currentRoutingProducts);
                        }
                        currentRoutingProducts.AddRoutingProduct(routingProduct);
                    }
                    return routingProductsByNumberPlan;
                });

            }
        }

        class RoutingProducts
        {
            public RoutingProducts()
            {
                RoutingProductIdsWithAllZones = new HashSet<int>();
                RoutingProductIdsWithSpecificZones = new Dictionary<long, List<int>>();
            }

            public void AddRoutingProduct(RoutingProduct routingProduct)
            {
                if (routingProduct.Settings != null)
                    switch (routingProduct.Settings.ZoneRelationType)
                    {
                        case RoutingProductZoneRelationType.AllZones:
                            RoutingProductIdsWithAllZones.Add(routingProduct.RoutingProductId);
                            break;
                        case RoutingProductZoneRelationType.SpecificZones:
                            foreach (var saleZone in routingProduct.Settings.Zones)
                            {
                                List<int> routingProductsWithSpecificZones;
                                if (!RoutingProductIdsWithSpecificZones.TryGetValue(saleZone.ZoneId, out routingProductsWithSpecificZones))
                                {
                                    RoutingProductIdsWithSpecificZones.Add(saleZone.ZoneId, routingProductsWithSpecificZones);
                                }
                                routingProductsWithSpecificZones.Add(routingProduct.RoutingProductId);
                            }
                            break;
                    }
            }

            public HashSet<int> GetRoutingProductsByZoneId(long zoneId)
            {
                List<int> routingProductIds = new List<int>();
                if (!RoutingProductIdsWithSpecificZones.TryGetValue(zoneId, out routingProductIds))
                    routingProductIds = new List<int>();

                routingProductIds.AddRange(RoutingProductIdsWithAllZones);

                return new HashSet<int>(routingProductIds);
            }

            HashSet<int> RoutingProductIdsWithAllZones { get; set; }
            Dictionary<long, List<int>> RoutingProductIdsWithSpecificZones { get; set; }
        }

        #endregion
    }
}
