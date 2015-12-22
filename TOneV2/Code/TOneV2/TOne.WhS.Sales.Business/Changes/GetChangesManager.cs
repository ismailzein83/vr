using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class GetChangesManager
    {
        SalePriceListOwnerType _ownerType;
        int _ownerId;
        int? _sellingProductId;
        DateTime _effectiveOn;
        Changes _changes;

        public GetChangesManager(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _sellingProductId = sellingProductId;
            _effectiveOn = effectiveOn;

            IRatePlanDataManager ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            _changes = ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
        }

        public ChangesSummary GetChangesSummary()
        {
            ChangesSummary summary = new ChangesSummary();

            if (_changes != null)
            {
                if (_changes.DefaultChanges != null)
                {
                    if (_changes.DefaultChanges.NewDefaultRoutingProduct != null)
                    {
                        RoutingProductManager routingProductManager = new RoutingProductManager();
                        summary.NewDefaultRoutingProductName = routingProductManager.GetRoutingProductName(_changes.DefaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId);
                    }
                }

                if (_changes.ZoneChanges != null)
                {
                    var newRates = _changes.ZoneChanges.MapRecords(itm => itm.NewRate, itm => itm.NewRate != null);
                    var rateChanges = _changes.ZoneChanges.MapRecords(itm => itm.RateChange, itm => itm.RateChange != null);
                    var newRoutingProducts = _changes.ZoneChanges.MapRecords(itm => itm.NewRoutingProduct, itm => itm.NewRoutingProduct != null);
                    var routingProductChanges = _changes.ZoneChanges.MapRecords(itm => itm.RoutingProductChange, itm => itm.RoutingProductChange != null);

                    if (newRates != null)
                    {
                        IEnumerable<SaleEntityZoneRate> currentZoneRates = GetCurrentRatesByZoneIds(newRates.MapRecords(itm => itm.ZoneId));

                        foreach (NewRate newRate in newRates)
                        {
                            var currentRate = currentZoneRates.FindRecord(itm => itm.Rate.ZoneId == newRate.ZoneId);

                            if (currentRate != null)
                            {
                                if (newRate.NormalRate > currentRate.Rate.NormalRate)
                                    summary.TotalRateIncreases++;
                                else if (newRate.NormalRate < currentRate.Rate.NormalRate)
                                    summary.TotalRateDecreases++;
                            }
                            else
                                summary.TotalNewRates++;
                        }
                    }

                    summary.TotalRateChanges = rateChanges != null ? rateChanges.Count() : 0;
                    summary.TotalNewZoneRoutingProducts = newRoutingProducts != null ? newRoutingProducts.Count() : 0;
                    summary.TotalZoneRoutingProductChanges = routingProductChanges != null ? routingProductChanges.Count() : 0;
                }
            }
            
            return summary;
        }

        #region Get Filtered Zone Rate Changes

        public IEnumerable<ZoneRateChangesDetail> GetFilteredZoneRateChanges()
        {
            List<ZoneRateChangesDetail> details = null;

            if (_changes != null && _changes.ZoneChanges != null)
            {
                IEnumerable<ZoneChanges> zoneChanges = _changes.ZoneChanges.FindAllRecords(itm => itm.NewRate != null || itm.RateChange != null);

                if (zoneChanges != null)
                {
                    details = new List<ZoneRateChangesDetail>();
                    SaleZoneManager saleZoneManager = new SaleZoneManager();

                    var zoneIds = zoneChanges.MapRecords(itm => itm.ZoneId);
                    var currentZoneRates = GetCurrentRatesByZoneIds(zoneIds);

                    foreach (ZoneChanges zoneItemChanges in zoneChanges)
                    {
                        ZoneRateChangesDetail detail = new ZoneRateChangesDetail()
                        {
                            ZoneId = zoneItemChanges.ZoneId,
                            ZoneName = saleZoneManager.GetSaleZoneName(zoneItemChanges.ZoneId)
                        };

                        var rate = currentZoneRates.FindRecord(itm => itm.Rate.ZoneId == zoneItemChanges.ZoneId);

                        if (rate != null)
                        {
                            detail.CurrentRate = rate.Rate.NormalRate;
                            detail.IsCurrentRateInherited = (_ownerType == SalePriceListOwnerType.Customer && rate.Source == SalePriceListOwnerType.SellingProduct);
                        }

                        if (zoneItemChanges.NewRate != null)
                        {
                            detail.NewRate = zoneItemChanges.NewRate.NormalRate;
                            detail.EffectiveOn = zoneItemChanges.NewRate.BED;
                            detail.EffectiveUntil = zoneItemChanges.NewRate.EED;
                        }
                        else if (zoneItemChanges.RateChange != null)
                            detail.EffectiveOn = zoneItemChanges.RateChange.EED;

                        if (detail.CurrentRate != null && detail.NewRate != null)
                        {
                            if (detail.NewRate > detail.CurrentRate)
                                detail.ChangeType = Entities.RateChangeType.Increase;
                            else if (detail.NewRate < detail.CurrentRate)
                                detail.ChangeType = Entities.RateChangeType.Decrease;
                        }
                        else if (detail.NewRate != null)
                            detail.ChangeType = Entities.RateChangeType.New;
                        else
                            detail.ChangeType = Entities.RateChangeType.Close;

                        details.Add(detail);
                    }
                }
            }

            return details;
        }

        IEnumerable<SaleEntityZoneRate> GetCurrentRatesByZoneIds(IEnumerable<long> zoneIds)
        {
            List<SaleEntityZoneRate> rates = null;

            if (zoneIds != null && _sellingProductId != null)
            {
                rates = new List<SaleEntityZoneRate>();
                int sellingProductId = (int)_sellingProductId;

                SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(_effectiveOn));

                foreach (long zoneId in zoneIds)
                {
                    SaleEntityZoneRate rate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                        rateLocator.GetSellingProductZoneRate(sellingProductId, zoneId) : rateLocator.GetCustomerZoneRate(_ownerId, sellingProductId, zoneId);

                    if (rate != null)
                        rates.Add(rate);
                }
            }

            return rates;
        }
        
        #endregion

        #region Get Filtered Zone Routing Product Changes

        public IEnumerable<ZoneRoutingProductChangesDetail> GetFilteredZoneRoutingProductChanges()
        {
            List<ZoneRoutingProductChangesDetail> details = null;

            if (_changes != null && _changes.ZoneChanges != null)
            {
                IEnumerable<ZoneChanges> zoneChanges = _changes.ZoneChanges.FindAllRecords(itm => itm.NewRoutingProduct != null || itm.RoutingProductChange != null);

                if (zoneChanges != null)
                {
                    details = new List<ZoneRoutingProductChangesDetail>();
                    SaleZoneManager saleZoneManager = new SaleZoneManager();
                    RoutingProductManager routingProductManager = new RoutingProductManager();

                    IEnumerable<SaleEntityZoneRoutingProduct> currentRoutingProducts = GetCurrentRoutingProductsByZoneIds(zoneChanges.MapRecords(itm => itm.ZoneId));

                    foreach (ZoneChanges zoneItemChanges in zoneChanges)
                    {
                        ZoneRoutingProductChangesDetail detail = new ZoneRoutingProductChangesDetail()
                        {
                            ZoneId = zoneItemChanges.ZoneId,
                            ZoneName = saleZoneManager.GetSaleZoneName(zoneItemChanges.ZoneId)
                        };

                        SaleEntityZoneRoutingProduct currentRoutingProduct = currentRoutingProducts.FindRecord(itm => (zoneItemChanges.NewRoutingProduct != null && itm.RoutingProductId == zoneItemChanges.NewRoutingProduct.ZoneRoutingProductId) || (zoneItemChanges.RoutingProductChange != null && itm.RoutingProductId == zoneItemChanges.RoutingProductChange.ZoneRoutingProductId));

                        if (currentRoutingProduct != null)
                        {
                            detail.CurrentRoutingProductName = routingProductManager.GetRoutingProductName(currentRoutingProduct.RoutingProductId);
                            detail.IsCurrentRoutingProductInherited = !IsZoneRoutingProductEditable(currentRoutingProduct.Source);
                        }

                        if (zoneItemChanges.NewRoutingProduct != null)
                        {
                            detail.NewRoutingProductName = routingProductManager.GetRoutingProductName(zoneItemChanges.NewRoutingProduct.ZoneRoutingProductId);
                            detail.EffectiveOn = zoneItemChanges.NewRoutingProduct.BED;
                        }
                        else if (zoneItemChanges.RoutingProductChange != null)
                        {
                            detail.NewRoutingProductName = "(Default)"; // This isn't expressive, but it's necessary
                            detail.EffectiveOn = zoneItemChanges.RoutingProductChange.EED;
                        }

                        details.Add(detail);
                    }
                }
            }

            return details;
        }

        IEnumerable<SaleEntityZoneRoutingProduct> GetCurrentRoutingProductsByZoneIds(IEnumerable<long> zoneIds)
        {
            List<SaleEntityZoneRoutingProduct> currentRoutingProducts = null;

            if (zoneIds != null && _sellingProductId != null)
            {
                currentRoutingProducts = new List<SaleEntityZoneRoutingProduct>();
                int sellingProductId = (int)_sellingProductId;

                SaleEntityZoneRoutingProductLocator routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(_effectiveOn));

                foreach (long zoneId in zoneIds)
                {
                    SaleEntityZoneRoutingProduct routingProduct = _ownerType == SalePriceListOwnerType.SellingProduct ?
                        routingProductLocator.GetSellingProductZoneRoutingProduct(sellingProductId, zoneId) : routingProductLocator.GetCustomerZoneRoutingProduct(_ownerId, sellingProductId, zoneId);

                    if (routingProduct != null)
                        currentRoutingProducts.Add(routingProduct);
                }
            }

            return currentRoutingProducts;
        }

        bool IsZoneRoutingProductEditable(SaleEntityZoneRoutingProductSource rpSource)
        {
            if (_ownerType == SalePriceListOwnerType.SellingProduct && rpSource == SaleEntityZoneRoutingProductSource.ProductZone)
                return true;
            else if (_ownerType == SalePriceListOwnerType.Customer && rpSource == SaleEntityZoneRoutingProductSource.CustomerZone)
                return true;
            return false;
        }
        
        #endregion
    }
}
