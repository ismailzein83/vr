using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RoutingProductManager
    {
        #region Public Methods
        public Dictionary<RoutingProductOwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>> GetAllCachedSaleZoneRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject("GetAllCachedSaleZoneRoutingProducts", () =>
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                var allSaleZoneRoutingProducts = saleEntityRoutingProductDataManager.GetAllZoneRoutingProducts();
                Dictionary<RoutingProductOwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>> rslt = new Dictionary<RoutingProductOwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>>();

                if (allSaleZoneRoutingProducts != null)
                {
                    foreach (var szRP in allSaleZoneRoutingProducts)
                    {
                        rslt.GetOrCreateItem(new RoutingProductOwnerKey { OwnerId = szRP.OwnerId, OwnerType = szRP.OwnerType }).GetOrCreateItem(szRP.SaleZoneId).Add(szRP);
                    }
                }
                return rslt;
            });
        }

        public Dictionary<RoutingProductOwnerKey, List<DefaultRoutingProduct>> GetAllCachedDefaultRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject("GetAllCachedDefaultRoutingProducts", () =>
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                var allSaleZoneRoutingProducts = saleEntityRoutingProductDataManager.GetAllDefaultRoutingProducts();
                Dictionary<RoutingProductOwnerKey, List<DefaultRoutingProduct>> rslt = new Dictionary<RoutingProductOwnerKey, List<DefaultRoutingProduct>>();

                if (allSaleZoneRoutingProducts != null)
                {
                    foreach (var szRP in allSaleZoneRoutingProducts)
                    {
                        rslt.GetOrCreateItem(new RoutingProductOwnerKey { OwnerId = szRP.OwnerId, OwnerType = szRP.OwnerType }).Add(szRP);
                    }
                }
                return rslt;
            });
        }




        public Vanrise.Entities.IDataRetrievalResult<RoutingProductDetail> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            var allRoutingProducts = GetAllRoutingProducts();

            Func<RoutingProduct, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId));

            ResultProcessingHandler<RoutingProductDetail> handler = new ResultProcessingHandler<RoutingProductDetail>()
            {
                ExportExcelHandler = new RoutingProductExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(RoutingProductLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRoutingProducts.ToBigResult(input, filterExpression, RoutingProductDetailMapper), handler);
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlanId)
        {
            var routingProducts = GetAllRoutingProducts();
            return routingProducts.MapRecords(RoutingProductInfoMapper, x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public IEnumerable<RoutingProduct> GetRoutingProductsBySellingNumberPlan(int sellingNumberPlanId)
        {
            var routingProducts = GetAllRoutingProducts();
            return routingProducts.FindAllRecords(x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductInfo(RoutingProductInfoFilter filter)
        {
            var routingProducts = GetAllRoutingProducts();

            int? filterSellingNumberPlanId = null;
            HashSet<int> filteredRoutingProductIds = null;
            int? excludedRoutingProductId = null;

            Func<RoutingProduct, bool> filterAssignableToSaleEntity = null;

            Func<RoutingProduct, bool> filterPredicate = (rp) =>
            {
                if (filterSellingNumberPlanId.HasValue && rp.SellingNumberPlanId != filterSellingNumberPlanId.Value)
                    return false;

                if (excludedRoutingProductId.HasValue && rp.RoutingProductId == excludedRoutingProductId.Value)
                    return false;

                if (filteredRoutingProductIds != null && !filteredRoutingProductIds.Contains(rp.RoutingProductId))
                    return false;

                if (filterAssignableToSaleEntity != null && !filterAssignableToSaleEntity(rp))
                    return false;

                return true;
            };

            if (filter != null)
            {
                filterSellingNumberPlanId = filter.SellingNumberPlanId;

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

            return routingProducts.MapRecords(RoutingProductInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }

        public InsertOperationOutput<RoutingProductDetail> AddRoutingProduct(RoutingProduct routingProduct)
        {
            ValidateRoutingProductToAdd(routingProduct);

            InsertOperationOutput<RoutingProductDetail> insertOperationOutput = new InsertOperationOutput<RoutingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routingProductId = -1;

            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(routingProduct, out routingProductId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routingProduct.RoutingProductId = routingProductId;
                VRActionLogger.Current.TrackAndLogObjectAdded(RoutingProductLoggableEntity.Instance, routingProduct);
                insertOperationOutput.InsertedObject = RoutingProductDetailMapper(routingProduct);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<RoutingProductDetail> UpdateRoutingProduct(RoutingProductToEdit routingProduct)
        {
            if (CheckIfRoutingProductHasRelatedSaleEntities(routingProduct.RoutingProductId))
            {
                RoutingProduct currentRoutingProduct = this.GetRoutingProduct(routingProduct.RoutingProductId);
                if (currentRoutingProduct.Settings.ZoneRelationType != routingProduct.Settings.ZoneRelationType ||
                    !currentRoutingProduct.Settings.CheckIfDefaultServiceIdsAreSame(routingProduct.Settings) ||
                    !currentRoutingProduct.Settings.CheckIfZonesAreSame(routingProduct.Settings) ||
                    !currentRoutingProduct.Settings.CheckIfZoneServicesAreSame(routingProduct.Settings))
                    throw new Vanrise.Entities.VRBusinessException(string.Format("Cannot edit Routing Product '{0}' because it is in use.", routingProduct.Name));
            }

            ValidateRoutingProductToEdit(routingProduct);

            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            bool updateActionSucc = dataManager.Update(routingProduct);
            var updateOperationOutput = new UpdateOperationOutput<RoutingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(RoutingProductLoggableEntity.Instance, GetRoutingProduct(routingProduct.RoutingProductId));
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RoutingProductDetailMapper(this.GetRoutingProduct(routingProduct.RoutingProductId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteRoutingProduct(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }



        public RoutingProduct GetRoutingProduct(int routingProductId, bool isViewedFromUI)
        {
            var allRoutingProducts = GetAllRoutingProducts();
            var routingProduct = allRoutingProducts.GetRecord(routingProductId);
            if (routingProduct != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(RoutingProductLoggableEntity.Instance, routingProduct);
            return routingProduct;
        }

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            return GetRoutingProduct(routingProductId, false);
        }

        public RoutingProductEditorRuntime GetRoutingProductEditorRuntime(int routingProductId)
        {
            RoutingProductEditorRuntime routingProductEditorRuntime = new RoutingProductEditorRuntime();

            routingProductEditorRuntime.Entity = GetRoutingProduct(routingProductId);

            var zoneServices = routingProductEditorRuntime.Entity.Settings.ZoneServices;
            if (zoneServices == null)
                return routingProductEditorRuntime;
            HashSet<long> zoneIds;
            HashSet<int> serviceIds;
            GetZoneServicesData(zoneServices, out zoneIds, out serviceIds);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            routingProductEditorRuntime.ZoneNames = saleZoneManager.GetSaleZonesNames(zoneIds);

            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
            routingProductEditorRuntime.ServiceNames = zoneServiceConfigManager.GetZoneServicesNamesDict(serviceIds);

            return routingProductEditorRuntime;
        }

        private void GetZoneServicesData(List<RoutingProductZoneService> zoneServices, out HashSet<long> zoneIds, out HashSet<int> serviceIds)
        {
            List<long> tempZoneIds = new List<long>();
            List<int> tempServiceIds = new List<int>();

            foreach (RoutingProductZoneService zoneService in zoneServices)
            {
                tempZoneIds.AddRange(zoneService.ZoneIds);
                tempServiceIds.AddRange(zoneService.ServiceIds);
            }
            zoneIds = tempZoneIds.ToHashSet<long>();
            serviceIds = tempServiceIds.ToHashSet<int>();
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

        public Dictionary<int, RoutingProduct> GetAllRoutingProductsByIds(IEnumerable<int> routingProductIds)
        {
            if (routingProductIds == null)
                return null;

            Dictionary<int, RoutingProduct> result = new Dictionary<int, RoutingProduct>();

            var routingProducts = GetAllRoutingProducts();
            if (routingProducts != null)
            {
                var matchingRoutingProducts = routingProducts.FindAllRecords(x => routingProductIds.Contains(x.Key));

                if (matchingRoutingProducts != null)
                {
                    result = matchingRoutingProducts.ToDictionary(itm => itm.Key, itm => itm.Value);
                }
            }
            return result;
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

            if (routingProduct != null)
                return routingProduct.Name;

            return null;
        }

        public IEnumerable<int> GetDefaultServiceIds(int routingProductId)
        {
            RoutingProduct routingProduct = GetRoutingProduct(routingProductId);
            if (routingProduct == null)
                return null;
            if (routingProduct.Settings == null)
                throw new NullReferenceException("routingProduct.Settings");
            return routingProduct.Settings.DefaultServiceIds;
        }

        public IEnumerable<int> GetZoneServiceIds(int routingProductId, long zoneId)
        {
            RoutingProduct routingProduct = GetRoutingProduct(routingProductId);
            if (routingProduct == null)
                return null;
            if (routingProduct.Settings == null)
                throw new NullReferenceException("routingProduct.Settings");
            return routingProduct.Settings.GetZoneServices(zoneId);
        }

        public bool CheckIfRoutingProductHasRelatedSaleEntities(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            return dataManager.CheckIfRoutingProductHasRelatedSaleEntities(routingProductId);
        }

        #endregion

        #region Validation Methods

        void ValidateRoutingProductToAdd(RoutingProduct routingProduct)
        {
            var sellingNumberPlanManager = new SellingNumberPlanManager();
            SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(routingProduct.SellingNumberPlanId);
            if (sellingNumberPlan == null)
                throw new MissingArgumentValidationException(String.Format("SellingNumberPlan '{0}' of RoutingProduct does not exist", routingProduct.SellingNumberPlanId));

            ValidateRoutingProduct(routingProduct.Name, routingProduct.Settings, routingProduct.SellingNumberPlanId);
        }

        void ValidateRoutingProductToEdit(RoutingProductToEdit routingProduct)
        {
            RoutingProduct routingProductEntity = this.GetRoutingProduct(routingProduct.RoutingProductId);
            if (routingProductEntity == null)
                throw new DataIntegrityValidationException(String.Format("RoutingProduct '{0}' does not exit", routingProduct.RoutingProductId));

            ValidateRoutingProduct(routingProduct.Name, routingProduct.Settings, routingProductEntity.SellingNumberPlanId);
        }

        void ValidateRoutingProduct(string rpName, RoutingProductSettings rpSettings, int sellingNumberPlanId)
        {
            if (String.IsNullOrWhiteSpace(rpName))
                throw new MissingArgumentValidationException("RoutingProduct.Name");

            if (rpSettings == null)
                throw new MissingArgumentValidationException("RoutingProduct.Settings");

            if (rpSettings.ZoneRelationType == RoutingProductZoneRelationType.AllZones)
            {
                if (rpSettings.Zones != null)
                    throw new DataIntegrityValidationException(String.Format("RoutingProduct.Settings.Zones must be null when RoutingProduct.Settings.ZoneRelationType = AllZones"));
            }
            else if (rpSettings.ZoneRelationType == RoutingProductZoneRelationType.SpecificZones)
            {
                if (rpSettings.Zones == null || rpSettings.Zones.Count() == 0)
                    throw new MissingArgumentValidationException("RoutingProduct.Settings.Zones");

                var saleZoneManager = new SaleZoneManager();

                foreach (RoutingProductZone rpZone in rpSettings.Zones)
                {
                    SaleZone saleZone = saleZoneManager.GetSaleZone(rpZone.ZoneId);

                    if (saleZone == null)
                        throw new DataIntegrityValidationException(String.Format("SaleZone '{0}' does not exist", rpZone.ZoneId));

                    if (saleZone.SellingNumberPlanId != sellingNumberPlanId)
                        throw new DataIntegrityValidationException(String.Format("The SellingNumberPlanId '{0}' of SaleZone '{1}' does not match the SellingNumberPlanId '{2}' of the RoutingProduct", saleZone.SellingNumberPlanId, saleZone.Name, sellingNumberPlanId));
                }
            }

            if (rpSettings.SupplierRelationType == RoutingProductSupplierRelationType.AllSuppliers)
            {
                if (rpSettings.Suppliers != null)
                    throw new DataIntegrityValidationException(String.Format("RoutingProduct.Settings.Suppliers must be null when RoutingProduct.Settings.SupplierRelationType = AllSuppliers"));
            }
            else if (rpSettings.SupplierRelationType == RoutingProductSupplierRelationType.SpecificSuppliers)
            {
                if (rpSettings.Suppliers == null || rpSettings.Suppliers.Count == 0)
                    throw new MissingArgumentValidationException("RoutingProduct.Settings.Suppliers");

                var carrierAccountManager = new CarrierAccountManager();

                foreach (RoutingProductSupplier rpSupplier in rpSettings.Suppliers)
                {
                    CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(rpSupplier.SupplierId);

                    if (supplier == null || (supplier.AccountType != CarrierAccountType.Supplier && supplier.AccountType != CarrierAccountType.Exchange))
                        throw new DataIntegrityValidationException(String.Format("Supplier '{0}' does not exit", rpSupplier.SupplierId));
                }
            }
        }

        #endregion

        #region Private Methods

        private RoutingProductInfo RoutingProductInfoMapper(RoutingProduct routingProduct)
        {
            var routingProductInfo = new RoutingProductInfo()
            {
                RoutingProductId = routingProduct.RoutingProductId,
                Name = routingProduct.Name,
                SellingNumberPlanId = routingProduct.SellingNumberPlanId
            };
            if (routingProduct.Settings == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Settings of RoutingProduct '{0}' were not found", routingProduct.RoutingProductId));
            routingProductInfo.IsDefinedForAllZones = (routingProduct.Settings.ZoneRelationType == RoutingProductZoneRelationType.AllZones);
            return routingProductInfo;
        }

        private RoutingProductDetail RoutingProductDetailMapper(RoutingProduct routingProduct)
        {
            if (routingProduct == null)
                return null;

            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
            SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(routingProduct.SellingNumberPlanId);

            return new RoutingProductDetail()
            {
                Entity = routingProduct,
                SellingNumberPlan = sellingNumberPlan != null ? sellingNumberPlan.Name : null
            };
        }

        #endregion

        #region Private Classes
        private class RoutingProductExcelExportHandler : ExcelExportHandler<RoutingProductDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RoutingProductDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Routing Products",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan", Width = 50 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlan });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
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
                                    routingProductsWithSpecificZones = new List<int>();
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


        private class RoutingProductLoggableEntity : VRLoggableEntityBase
        {
            public static RoutingProductLoggableEntity Instance = new RoutingProductLoggableEntity();

            private RoutingProductLoggableEntity()
            {

            }

            static RoutingProductManager s_routingProductManagerManager = new RoutingProductManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_RoutingProduct"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Routing Product"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_RoutingProduct_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                RoutingProduct routingProduct = context.Object.CastWithValidate<RoutingProduct>("context.Object");
                return routingProduct.RoutingProductId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                RoutingProduct routingProduct = context.Object.CastWithValidate<RoutingProduct>("context.Object");
                return s_routingProductManagerManager.GetRoutingProductName(routingProduct.RoutingProductId);

            }
        }

        #endregion
    }
    public struct RoutingProductOwnerKey
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
    }
}
