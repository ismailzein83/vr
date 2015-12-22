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
        RoutingProductManager _routingProductManager;

        IEnumerable<NewZoneRoutingProduct> _newZoneRoutingProducts;
        IEnumerable<ZoneRoutingProductChange> _zoneRoutingProductChanges;

        SaleEntityZoneRoutingProduct _currentZoneRoutingProduct;
        SaleEntityZoneRoutingProduct _currentDefaultRoutingProduct;
        NewDefaultRoutingProduct _newDefaultRoutingProduct;
        DefaultRoutingProductChange _defaultRoutingProductChange;
        SaleEntityZoneRoutingProduct _currentSellingProductDefaultRoutingProduct;

        public ZoneRoutingProductSetter(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn, Changes changes)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _sellingProductId = sellingProductId;
            _effectiveOn = effectiveOn;
            _routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(_effectiveOn));
            _routingProductManager = new RoutingProductManager();

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

            _currentDefaultRoutingProduct = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _routingProductLocator.GetSellingProductDefaultRoutingProduct(_ownerId) :
                _routingProductLocator.GetCustomerDefaultRoutingProduct(_ownerId, (int)_sellingProductId);

            _currentSellingProductDefaultRoutingProduct = (ownerType == SalePriceListOwnerType.Customer && _currentDefaultRoutingProduct != null && _currentDefaultRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerDefault) ?
                _routingProductLocator.GetSellingProductDefaultRoutingProduct((int)sellingProductId) : _currentDefaultRoutingProduct;
        }

        public void SetZoneRoutingProduct(ZoneItem zoneItem)
        {
            _currentZoneRoutingProduct = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
                _routingProductLocator.GetSellingProductZoneRoutingProduct(_ownerId, zoneItem.ZoneId) :
                _routingProductLocator.GetCustomerZoneRoutingProduct(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

            if (_currentZoneRoutingProduct != null)
            {
                zoneItem.CurrentRoutingProductId = _currentZoneRoutingProduct.RoutingProductId;
                zoneItem.CurrentRoutingProductName = _routingProductManager.GetRoutingProductName(_currentZoneRoutingProduct.RoutingProductId);
                zoneItem.CurrentRoutingProductBED = _currentZoneRoutingProduct.BED;
                zoneItem.CurrentRoutingProductEED = _currentZoneRoutingProduct.EED;
                zoneItem.IsCurrentRoutingProductEditable = ((_ownerType == SalePriceListOwnerType.SellingProduct && _currentZoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone) || (_ownerType == SalePriceListOwnerType.Customer && _currentZoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone));
            }

            SetZoneRoutingProductChanges(zoneItem);
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
            if (zoneItem.NewRoutingProductId != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.NewRoutingProductId);

            else if (zoneItem.RoutingProductChangeEED != null)
                SetZoneEffectiveRoutingProductToInherited(zoneItem);

            // If the current routing product isn't inherited
            else if (zoneItem.CurrentRoutingProductId != null && (zoneItem.IsCurrentRoutingProductEditable != null && (bool)zoneItem.IsCurrentRoutingProductEditable))
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.CurrentRoutingProductId);
            
            else
                SetZoneEffectiveRoutingProductToInherited(zoneItem);
        }

        void SetZoneEffectiveRoutingProductToInherited(ZoneItem zoneItem)
        {
            if (_newDefaultRoutingProduct != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, _newDefaultRoutingProduct.DefaultRoutingProductId);
            
            else if (_ownerType == SalePriceListOwnerType.SellingProduct && _currentDefaultRoutingProduct != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, _currentDefaultRoutingProduct.RoutingProductId);

            else if (_ownerType == SalePriceListOwnerType.Customer)
            {
                if (_defaultRoutingProductChange != null && _currentSellingProductDefaultRoutingProduct != null)
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _currentSellingProductDefaultRoutingProduct.RoutingProductId);

                else if (_currentDefaultRoutingProduct != null && _currentDefaultRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerDefault)
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _currentDefaultRoutingProduct.RoutingProductId);

                else if (_currentZoneRoutingProduct != null && _currentZoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone)
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _currentZoneRoutingProduct.RoutingProductId);

                else if (_currentSellingProductDefaultRoutingProduct != null)
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _currentSellingProductDefaultRoutingProduct.RoutingProductId);
            }
        }

        void SetZoneEffectiveRoutingProductProperties(ZoneItem zoneItem, int effectiveRoutingProductId)
        {
            zoneItem.EffectiveRoutingProductId = effectiveRoutingProductId;
            zoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(effectiveRoutingProductId);
        }
    }
}
