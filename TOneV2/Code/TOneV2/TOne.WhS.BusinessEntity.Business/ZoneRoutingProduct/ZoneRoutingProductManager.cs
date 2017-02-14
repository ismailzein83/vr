using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneRoutingProductManager
    {
        #region public Methods
        public IDataRetrievalResult<ZoneRoutingProductDetail> GetFilteredZoneRoutingProducts(DataRetrievalInput<ZoneRoutingProductQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ZoneRoutingProductHandler());
        }
        public SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            object saleAreaSettingData = GetSettingData(Constants.SaleAreaSettings);
            var saleAreaSettings = saleAreaSettingData as SaleAreaSettingsData;
            if (saleAreaSettings == null)
                throw new NullReferenceException("saleAreaSettings");
            return saleAreaSettings;
        }
        #endregion

        #region private Methods
        private object GetSettingData(string settingType)
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(settingType);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            return setting.Data;
        }
        private class ZoneRoutingProductHandler : BigDataRequestHandler<ZoneRoutingProductQuery, ZoneRoutingProduct, ZoneRoutingProductDetail>
        {
            RoutingProductManager _routingProductManager = new RoutingProductManager();
            SaleZoneManager _saleZoneManager = new SaleZoneManager();

            #region mapper
            private ZoneRoutingProduct ZoneRoutingProductMapper(SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct, long zoneId, IEnumerable<int> serviceIds, bool IsInherited)
            {
                return new ZoneRoutingProduct
                {
                    BED = saleEntityZoneRoutingProduct.BED,
                    EED = saleEntityZoneRoutingProduct.EED,
                    ZoneRoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId,
                    ZoneId = zoneId,
                    ServiceIds = serviceIds.ToList(),
                    IsInherited = IsInherited
                };
            }
            public override ZoneRoutingProductDetail EntityDetailMapper(ZoneRoutingProduct entity)
            {
                return new ZoneRoutingProductDetail
                {
                    Entity = entity,
                    RoutingProductName = _routingProductManager.GetRoutingProductName(entity.ZoneRoutingProductId),
                    ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId)
                };
            }

            #endregion

            #region public Methods
            public override IEnumerable<ZoneRoutingProduct> RetrieveAllData(DataRetrievalInput<ZoneRoutingProductQuery> input)
            {
                List<ZoneRoutingProduct> zoneRoutingProducts = new List<ZoneRoutingProduct>();
                IEnumerable<SaleZone> saleZones = (input.Query.SellingNumberPlanId.HasValue) ?
                    _saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId.Value, input.Query.EffectiveOn, false) :
                    _saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.EffectiveOn, false);
                var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.Query.EffectiveOn));
                if (saleZones == null) return zoneRoutingProducts;
                var filteredSaleZone = saleZones.FindAllRecords(sz => input.Query.ZonesIds == null || input.Query.ZonesIds.Contains(sz.SaleZoneId));
                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    foreach (SaleZone saleZone in filteredSaleZone)
                    {
                        SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct =
                            routingProductLocator.GetSellingProductZoneRoutingProduct(input.Query.OwnerId,
                                saleZone.SaleZoneId);
                        if (saleEntityZoneRoutingProduct != null)
                        {
                            var serviceIds = _routingProductManager.GetZoneServiceIds(
                                saleEntityZoneRoutingProduct.RoutingProductId, saleZone.SaleZoneId);
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(saleEntityZoneRoutingProduct, saleZone.SaleZoneId, serviceIds, false);
                            zoneRoutingProducts.Add(routingProduct);
                        }
                    }
                }
                else
                {
                    var customerSellingProductManager = new CustomerSellingProductManager();
                    var sellingProductId =
                        customerSellingProductManager.GetEffectiveSellingProductId(input.Query.OwnerId,
                            input.Query.EffectiveOn, false);
                    if (!sellingProductId.HasValue)
                        throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to any selling product", input.Query.OwnerId));

                    foreach (SaleZone saleZone in filteredSaleZone)
                    {
                        SaleEntityZoneRoutingProduct zoneRoutingProduct =
                            routingProductLocator.GetCustomerZoneRoutingProduct(input.Query.OwnerId,
                                sellingProductId.Value, saleZone.SaleZoneId);
                        if (zoneRoutingProduct != null)
                        {
                            var serviceIds = _routingProductManager.GetZoneServiceIds(
                                zoneRoutingProduct.RoutingProductId, saleZone.SaleZoneId);
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(zoneRoutingProduct,
                                saleZone.SaleZoneId, serviceIds,
                                zoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);
                            zoneRoutingProducts.Add(routingProduct);
                        }
                    }
                }
                return zoneRoutingProducts;
            }
            #endregion
        }
        #endregion
    }
}
