using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ZoneRoutingProductSetter
    {
        SalePriceListOwnerType _ownerType;
        int _ownerId;
        int? _sellingProductId;
        DateTime _effectiveOn;
        SaleEntityZoneRoutingProductLocator _routingProductLocator;

        IEnumerable<NewZoneRoutingProduct> _newZoneRoutingProducts;
        IEnumerable<ZoneRoutingProductChange> _zoneRoutingProductChanges;
        NewDefaultRoutingProduct _newDefaultRoutingProduct;
        DefaultRoutingProductChange _defaultRoutingProductChange;
        SaleEntityZoneRoutingProduct _currentDefaultRoutingProduct;

        public ZoneRoutingProductSetter(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn, Changes changes)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _sellingProductId = sellingProductId;
            _effectiveOn = effectiveOn;
            _routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(_effectiveOn));

            if (changes != null)
            {
                if (changes.DefaultChanges != null)
                {
                    _newDefaultRoutingProduct = changes.DefaultChanges.NewDefaultRoutingProduct;
                    _defaultRoutingProductChange = changes.DefaultChanges.DefaultRoutingProductChange;
                }

                if (changes.ZoneChanges != null)
                {
                    _newZoneRoutingProducts = changes.ZoneChanges.MapRecords(itm => itm.NewRoutingProduct, itm => itm.NewRoutingProduct != null);
                    _zoneRoutingProductChanges = changes.ZoneChanges.MapRecords(itm => itm.RoutingProductChange, itm => itm.RoutingProductChange != null);
                }
            }

            SetCurrentDefaultRoutingProduct();
        }

        void SetCurrentDefaultRoutingProduct()
        {
            _currentDefaultRoutingProduct = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _routingProductLocator.GetSellingProductDefaultRoutingProduct(_ownerId) :
                _routingProductLocator.GetCustomerDefaultRoutingProduct(_ownerId, (int)_sellingProductId);
        }

        public void SetZoneRoutingProduct(ZoneItem zoneItem)
        {
            SaleEntityZoneRoutingProduct routingProduct = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _routingProductLocator.GetSellingProductZoneRoutingProduct(_ownerId, zoneItem.ZoneId) :
                _routingProductLocator.GetCustomerZoneRoutingProduct(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

            if (routingProduct != null)
            {
                zoneItem.CurrentRoutingProductId = routingProduct.RoutingProductId;
                zoneItem.CurrentRoutingProductName = "None";
                zoneItem.CurrentRoutingProductBED = routingProduct.BED;
                zoneItem.CurrentRoutingProductEED = routingProduct.EED;
                zoneItem.IsCurrentRoutingProductEditable = ((_ownerType == SalePriceListOwnerType.SellingProduct && routingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone) || (_ownerType == SalePriceListOwnerType.Customer && routingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone));

                SetZoneRoutingProductChanges(zoneItem);
            }
            
            SetZoneEffectiveRoutingProduct(zoneItem);
        }

        void SetZoneRoutingProductChanges(ZoneItem zoneItem)
        {
            NewZoneRoutingProduct newZoneRoutingProduct = _newZoneRoutingProducts.FindRecord(itm => itm.ZoneId == zoneItem.ZoneId);
            ZoneRoutingProductChange zoneRoutingProductChange = _zoneRoutingProductChanges.FindRecord(itm => itm.ZoneRoutingProductId == zoneItem.CurrentRoutingProductId); // What if currentRoutingProductId = null?

            if (newZoneRoutingProduct != null)
            {
                zoneItem.NewRoutingProductId = newZoneRoutingProduct.ZoneRoutingProductId;
                zoneItem.NewRoutingProductBED = newZoneRoutingProduct.BED;
                zoneItem.NewRoutingProductEED = newZoneRoutingProduct.EED;
            }
            else if (zoneRoutingProductChange != null)
                zoneItem.RoutingProductChangeEED = zoneRoutingProductChange.EED;
        }

        void SetZoneEffectiveRoutingProduct(ZoneItem zoneItem)
        {
            if (_ownerType == SalePriceListOwnerType.SellingProduct)
                SetSellingProductZoneEffectiveRoutingProduct(zoneItem);
            else
                SetCustomerZoneEffectiveRoutingProduct(zoneItem);
        }

        void SetSellingProductZoneEffectiveRoutingProduct(ZoneItem zoneItem)
        {
            if (zoneItem.NewRoutingProductId != null) // New zone routing product
            {
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.NewRoutingProductId);
            }
            else if (zoneItem.RoutingProductChangeEED != null) // Zone routing product change
            {
                /* Thinking about it... */
            }
            else if (zoneItem.CurrentRoutingProductId != null && (zoneItem.IsCurrentRoutingProductEditable != null && (bool)zoneItem.IsCurrentRoutingProductEditable)) // Current zone routing product
            {
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.CurrentRoutingProductId);
            }
            else if (_newDefaultRoutingProduct != null)
            {
                SetZoneEffectiveRoutingProductProperties(zoneItem, _newDefaultRoutingProduct.DefaultRoutingProductId);
            }
            else if (_defaultRoutingProductChange != null)
            {
                /* Thinking about it */
            }
            else if (_currentDefaultRoutingProduct != null)
            {
                SetZoneEffectiveRoutingProductProperties(zoneItem, _currentDefaultRoutingProduct.RoutingProductId);
            }
        }

        void SetCustomerZoneEffectiveRoutingProduct(ZoneItem zoneItem)
        {

        }

        void SetZoneEffectiveRoutingProductProperties(ZoneItem zoneItem, int effectiveRoutingProductId)
        {
            zoneItem.EffectiveRoutingProductId = effectiveRoutingProductId;
            zoneItem.EffectiveRoutingProductName = "None";
        }
    }
}
