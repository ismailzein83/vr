using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanRPReadWithCache : BusinessEntity.Business.ISaleEntityRoutingProductReader
    {
        #region Fields / Constructors

        private BusinessEntity.Business.SaleEntityRoutingProductReadWithCache _reader;

        private SalePriceListOwnerType _ownerType;
        private int _ownerId;

        private BusinessEntity.Entities.DefaultRoutingProduct _defaultRoutingProduct;
        private BusinessEntity.Business.SaleZoneRoutingProductsByZone _routingProductsByZone;

        public RatePlanRPReadWithCache(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, Changes draft)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;

            _reader = new BusinessEntity.Business.SaleEntityRoutingProductReadWithCache(effectiveOn);

            SetDefaultRoutingProduct(draft != null ? draft.DefaultChanges : null);
            SetRoutingProductsByZone(draft != null ? draft.ZoneChanges : null);
        }

        #endregion

        public BusinessEntity.Entities.DefaultRoutingProduct GetDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId)
        {
            return IsSameOwner(ownerType, ownerId) ? _defaultRoutingProduct : _reader.GetDefaultRoutingProduct(ownerType, ownerId);
        }

        public BusinessEntity.Business.SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(SalePriceListOwnerType ownerType, int ownerId)
        {
            return IsSameOwner(ownerType, ownerId) ? _routingProductsByZone : _reader.GetRoutingProductsOnZones(ownerType, ownerId);
        }

        #region Private Methods

        private void SetDefaultRoutingProduct(DefaultChanges defaultDraft)
        {
            if (defaultDraft != null)
            {
                // TODO: Rename DefaultRPChange to DraftResetRP
                if (defaultDraft.DefaultRoutingProductChange != null)
                {
                    _defaultRoutingProduct = null;
                    return;
                }
                if (defaultDraft.NewDefaultRoutingProduct != null)
                {
                    _defaultRoutingProduct = new DefaultRoutingProduct()
                    {
                        OwnerType = _ownerType,
                        OwnerId = _ownerId,
                        RoutingProductId = defaultDraft.NewDefaultRoutingProduct.DefaultRoutingProductId,
                        BED = defaultDraft.NewDefaultRoutingProduct.BED,
                        EED = defaultDraft.NewDefaultRoutingProduct.EED
                    };
                    return;
                }
            }
            DefaultRoutingProduct existingDefaultRP = _reader.GetDefaultRoutingProduct(_ownerType, _ownerId);
            if (existingDefaultRP != null)
            {
                _defaultRoutingProduct = new DefaultRoutingProduct()
                {
                    OwnerType = _ownerType,
                    OwnerId = _ownerId,
                    RoutingProductId = existingDefaultRP.RoutingProductId,
                    BED = existingDefaultRP.BED,
                    EED = existingDefaultRP.EED
                };
            }
        }

        private void SetRoutingProductsByZone(IEnumerable<ZoneChanges> zoneDrafts)
        {
            var routingProductsByZone = new SaleZoneRoutingProductsByZone();

            SaleZoneRoutingProductsByZone existingRPsByZone = _reader.GetRoutingProductsOnZones(_ownerType, _ownerId);
            if (existingRPsByZone != null && existingRPsByZone.Values != null)
            {
                foreach (SaleZoneRoutingProduct existingZoneRP in existingRPsByZone.Values)
                {
                    var zoneRP = new SaleZoneRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingZoneRP.SaleEntityRoutingProductId,
                        OwnerType = _ownerType,
                        OwnerId = _ownerId,
                        SaleZoneId = existingZoneRP.SaleZoneId,
                        RoutingProductId = existingZoneRP.RoutingProductId,
                        BED = existingZoneRP.BED,
                        EED = existingZoneRP.EED
                    };
                    routingProductsByZone.Add(existingZoneRP.SaleZoneId, zoneRP);
                }
            }

            if (zoneDrafts != null)
            {
                foreach (ZoneChanges zoneDraft in zoneDrafts)
                {
                    if (zoneDraft.RoutingProductChange != null)
                    {
                        routingProductsByZone.Remove(zoneDraft.ZoneId);
                        continue;
                    }

                    SaleZoneRoutingProduct zoneRP;
                    routingProductsByZone.TryGetValue(zoneDraft.ZoneId, out zoneRP);

                    if (zoneDraft.NewRoutingProduct != null)
                    {
                        if (zoneRP == null)
                        {
                            zoneRP = new SaleZoneRoutingProduct()
                            {
                                OwnerType = _ownerType,
                                OwnerId = _ownerId,
                                SaleZoneId = zoneDraft.ZoneId,
                                RoutingProductId = zoneDraft.NewRoutingProduct.ZoneRoutingProductId,
                                BED = zoneDraft.NewRoutingProduct.BED,
                                EED = zoneDraft.NewRoutingProduct.EED
                            };
                            routingProductsByZone.Add(zoneDraft.ZoneId, zoneRP);
                        }
                        else
                        {
                            zoneRP.SaleEntityRoutingProductId = 0;
                            zoneRP.RoutingProductId = zoneDraft.NewRoutingProduct.ZoneRoutingProductId;
                            zoneRP.BED = zoneDraft.NewRoutingProduct.BED;
                            zoneRP.EED = zoneDraft.NewRoutingProduct.EED;
                        }
                    }
                }
            }

            _routingProductsByZone = routingProductsByZone;
        }

        private bool IsSameOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            return (ownerType == _ownerType && ownerId == _ownerId);
        }

        #endregion
    }
}
