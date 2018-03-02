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
    public class SaleEntityRoutingProductReadWithDraft : BusinessEntity.Business.ISaleEntityRoutingProductReader
    {
        #region Fields / Constructors

        private ISaleEntityRoutingProductReader _reader;

        private SalePriceListOwnerType _ownerType;
        private int _ownerId;
        private bool _defaultRoutingProductDraftExist;

        private BusinessEntity.Entities.DefaultRoutingProduct _defaultRoutingProduct;
        private BusinessEntity.Business.SaleZoneRoutingProductsByZone _routingProductsByZone;

        public SaleEntityRoutingProductReadWithDraft(SalePriceListOwnerType ownerType, int ownerId, Changes draft,ISaleEntityRoutingProductReader reader)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _reader = reader;

            SetDefaultRoutingProduct(draft != null ? draft.DefaultChanges : null);
            SetRoutingProductsByZone(draft != null ? draft.ZoneChanges : null);
        }
        
        #endregion

        public BusinessEntity.Entities.DefaultRoutingProduct GetDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, long? saleZoneId)
        {
            if (_defaultRoutingProductDraftExist == true)
                return _defaultRoutingProduct;
            return _reader.GetDefaultRoutingProduct(ownerType, ownerId, saleZoneId);
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
                    _defaultRoutingProductDraftExist = true;
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
                    _defaultRoutingProductDraftExist = true;
                    return;
                }
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
