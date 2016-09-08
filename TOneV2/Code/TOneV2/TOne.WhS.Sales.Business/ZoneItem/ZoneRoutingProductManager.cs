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
    public class ZoneRoutingProductManager
    {
        int _sellingProductId;
        int? _customerId;

        SaleEntityZoneRoutingProductLocator _routingProductLocator;
        DateTime _effectiveOn;
        RoutingProductManager _routingProductManager;

        IEnumerable<ZoneChanges> _zoneChanges;

        SaleEntityZoneRoutingProduct _customerCurrentDefaultRoutingProduct;
        SaleEntityZoneRoutingProduct _sellingProductCurrentDefaultRoutingProduct;

        DraftNewDefaultRoutingProduct _newDefaultRoutingProduct;
        DraftChangedDefaultRoutingProduct _defaultRoutingProductChange;

        public ZoneRoutingProductManager(int sellingProductId, int? customerId, DateTime effectiveOn, Changes changes)
        {
            ValidateOwner(sellingProductId, customerId);

            _sellingProductId = sellingProductId;
            _customerId = customerId;
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

                _zoneChanges = changes.ZoneChanges;
            }

            _sellingProductCurrentDefaultRoutingProduct = _routingProductLocator.GetSellingProductDefaultRoutingProduct(sellingProductId);

            if (customerId != null)
            {
                SaleEntityZoneRoutingProduct routingProduct = _routingProductLocator.GetCustomerDefaultRoutingProduct((int)customerId, sellingProductId);
                
                if (routingProduct != null && routingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerDefault)
                    _customerCurrentDefaultRoutingProduct = routingProduct;
            }
        }

        void ValidateOwner(object sellingProductId, object customerId)
        {
            if (sellingProductId == null && customerId == null)
                throw new Exception("Owner does not exist");
            else if (customerId != null && sellingProductId == null)
                throw new Exception("Customer is not associated with a selling product");
        }

        public void SetZoneRoutingProduct(ZoneItem zoneItem)
        {
            SaleEntityZoneRoutingProduct sellingProductCurrentZoneRoutingProduct = _routingProductLocator.GetSellingProductZoneRoutingProduct(_sellingProductId, zoneItem.ZoneId);
            SaleEntityZoneRoutingProduct customerCurrentZoneRoutingProduct = null;

            if (_customerId != null)
            {
                SaleEntityZoneRoutingProduct routingProduct = _routingProductLocator.GetCustomerZoneRoutingProduct((int)_customerId, _sellingProductId, zoneItem.ZoneId);
                
                if (routingProduct != null && routingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone)
                    customerCurrentZoneRoutingProduct = routingProduct;
            }

            SaleEntityZoneRoutingProduct currentZoneRoutingProduct = customerCurrentZoneRoutingProduct != null ? customerCurrentZoneRoutingProduct : sellingProductCurrentZoneRoutingProduct;
            SetCurrentRoutingProductProperties(zoneItem, currentZoneRoutingProduct);

            SetZoneRoutingProductChanges(zoneItem);
            SetZoneEffectiveRoutingProduct(zoneItem, sellingProductCurrentZoneRoutingProduct);
        }

        void SetCurrentRoutingProductProperties(ZoneItem zoneItem, SaleEntityZoneRoutingProduct currentZoneRoutingProduct)
        {
            if (currentZoneRoutingProduct != null)
            {
                zoneItem.CurrentRoutingProductId = currentZoneRoutingProduct.RoutingProductId;
                zoneItem.CurrentRoutingProductName = _routingProductManager.GetRoutingProductName(currentZoneRoutingProduct.RoutingProductId);
                zoneItem.CurrentRoutingProductBED = currentZoneRoutingProduct.BED;
                zoneItem.CurrentRoutingProductEED = currentZoneRoutingProduct.EED;

                zoneItem.IsCurrentRoutingProductEditable = (_customerId != null && currentZoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone) || (_customerId == null && currentZoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone);
            }
        }

        void SetZoneRoutingProductChanges(ZoneItem zoneItem)
        {
            if (_zoneChanges == null)
                return;

            ZoneChanges zoneItemChanges = _zoneChanges.FindRecord(item => item.ZoneId == zoneItem.ZoneId);

            if (zoneItemChanges == null)
                return;
            
            if (zoneItemChanges.NewRoutingProduct != null)
            {
                zoneItem.NewRoutingProductId = zoneItemChanges.NewRoutingProduct.ZoneRoutingProductId;
                zoneItem.NewRoutingProductBED = zoneItemChanges.NewRoutingProduct.BED;
                zoneItem.NewRoutingProductEED = zoneItemChanges.NewRoutingProduct.EED;
            }
            else if (zoneItemChanges.RoutingProductChange != null)
            {
                zoneItem.RoutingProductChangeEED = zoneItemChanges.RoutingProductChange.EED;
            }
        }

        void SetZoneEffectiveRoutingProduct(ZoneItem zoneItem, SaleEntityZoneRoutingProduct sellingProductCurrentZoneRoutingProduct)
        {
            if (zoneItem.NewRoutingProductId != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.NewRoutingProductId);

            else if (zoneItem.RoutingProductChangeEED != null)
                SetZoneEffectiveRoutingProductToInherited(zoneItem, sellingProductCurrentZoneRoutingProduct);

            // If the current routing product isn't inherited
            else if (zoneItem.IsCurrentRoutingProductEditable != null && (bool)zoneItem.IsCurrentRoutingProductEditable)
                SetZoneEffectiveRoutingProductProperties(zoneItem, (int)zoneItem.CurrentRoutingProductId); // If IsCurrentRoutingProductEditable != null, then CurrentRoutingProductId != null
            
            else
                SetZoneEffectiveRoutingProductToInherited(zoneItem, sellingProductCurrentZoneRoutingProduct);
        }

        void SetZoneEffectiveRoutingProductToInherited(ZoneItem zoneItem, SaleEntityZoneRoutingProduct sellingProductCurrentZoneRoutingProduct)
        {
            if (_newDefaultRoutingProduct != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, _newDefaultRoutingProduct.DefaultRoutingProductId);
            
            // If the owner is a selling product with a current default routing product
            else if (_customerId == null && _sellingProductCurrentDefaultRoutingProduct != null)
                SetZoneEffectiveRoutingProductProperties(zoneItem, _sellingProductCurrentDefaultRoutingProduct.RoutingProductId);

            else if (_customerId != null)
            {
                if (_defaultRoutingProductChange != null)
                {
                    if (_sellingProductCurrentDefaultRoutingProduct != null)
                        SetZoneEffectiveRoutingProductProperties(zoneItem, _sellingProductCurrentDefaultRoutingProduct.RoutingProductId);
                }
                else if (_customerCurrentDefaultRoutingProduct != null)
                {
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _customerCurrentDefaultRoutingProduct.RoutingProductId);
                }
                else if (sellingProductCurrentZoneRoutingProduct != null)
                {
                    SetZoneEffectiveRoutingProductProperties(zoneItem, sellingProductCurrentZoneRoutingProduct.RoutingProductId);
                }
                else if (_sellingProductCurrentDefaultRoutingProduct != null)
                {
                    SetZoneEffectiveRoutingProductProperties(zoneItem, _sellingProductCurrentDefaultRoutingProduct.RoutingProductId);
                }
            }
        }

        void SetZoneEffectiveRoutingProductProperties(ZoneItem zoneItem, int effectiveRoutingProductId)
        {
            zoneItem.EffectiveRoutingProductId = effectiveRoutingProductId;
            zoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(effectiveRoutingProductId);
        }
    }
}
