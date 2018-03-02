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
    public class ZoneRPManager
    {
        #region Fields / Constructors

        private SaleEntityZoneRoutingProductLocator _rpLocator;
        private SaleEntityZoneRoutingProductLocator _ratePlanRPLocator;
        private RoutingProductManager _rpManager;

        public ZoneRPManager(SalePriceListOwnerType ownerType, int ownerId, Changes draft, SaleEntityZoneRoutingProductLocator zoneRPLocator, SaleEntityRoutingProductReadByRateBED zoneRPReader)
        {
            _rpLocator = zoneRPLocator;
            _ratePlanRPLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithDraft(ownerType, ownerId, draft, zoneRPReader));
            _rpManager = new RoutingProductManager();
        }

        #endregion

        public void SetSellingProductZoneRP(ZoneItem zoneItem, int sellingProductId, ZoneChanges zoneDraft)
        {
            SaleEntityZoneRoutingProduct currentRP = _rpLocator.GetSellingProductZoneRoutingProduct(sellingProductId, zoneItem.ZoneId);
            SetZoneCurrentRPProperties(zoneItem, currentRP, SaleEntityZoneRoutingProductSource.ProductZone);

            SetDraftZoneRP(zoneItem, zoneDraft);
            SetSellingProductEffectiveZoneRP(zoneItem, sellingProductId);
        }

        public void SetCustomerZoneRP(ZoneItem zoneItem, int customerId, int sellingProductId, ZoneChanges zoneDraft)
        {
            SaleEntityZoneRoutingProduct currentRP = _rpLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneItem.ZoneId);
            SetZoneCurrentRPProperties(zoneItem, currentRP, SaleEntityZoneRoutingProductSource.CustomerZone);

            SetDraftZoneRP(zoneItem, zoneDraft);
            SetCustomerEffectiveZoneRP(zoneItem, customerId, sellingProductId);
        }

        #region Private Methods

        private void SetZoneCurrentRPProperties(ZoneItem zoneItem, SaleEntityZoneRoutingProduct currentRP, SaleEntityZoneRoutingProductSource targetSource)
        {
            if (currentRP == null)
                return;

            zoneItem.CurrentRoutingProductId = currentRP.RoutingProductId;
            zoneItem.CurrentRoutingProductName = _rpManager.GetRoutingProductName(currentRP.RoutingProductId);
            zoneItem.CurrentRoutingProductBED = currentRP.BED;
            zoneItem.CurrentRoutingProductEED = currentRP.EED;
            zoneItem.IsCurrentRoutingProductEditable = currentRP.Source == targetSource;

            zoneItem.CurrentServiceIds = _rpManager.GetZoneServiceIds(currentRP.RoutingProductId, zoneItem.ZoneId);
        }

        private void SetDraftZoneRP(ZoneItem zoneItem, ZoneChanges zoneDraft)
        {
            if (zoneDraft == null)
                return;
            if (zoneItem.NewRoutingProduct != null)
                return;
            zoneItem.NewRoutingProduct = zoneDraft.NewRoutingProduct;
            zoneItem.ResetRoutingProduct = zoneDraft.RoutingProductChange;
        }

        private void SetSellingProductEffectiveZoneRP(ZoneItem zoneItem, int sellingProductId)
        {
            if (zoneItem.EffectiveRoutingProductId.HasValue)
                return;
            SaleEntityZoneRoutingProduct ratePlanRP = _ratePlanRPLocator.GetSellingProductZoneRoutingProduct(sellingProductId, zoneItem.ZoneId);
            if (ratePlanRP != null)
                SetEffectiveRPProperties(zoneItem, ratePlanRP);
        }

        private void SetCustomerEffectiveZoneRP(ZoneItem zoneItem, int customerId, int sellingProductId)
        {
            if (zoneItem.EffectiveRoutingProductId.HasValue)
                return;
            SaleEntityZoneRoutingProduct ratePlanRP = _ratePlanRPLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneItem.ZoneId);
            if (ratePlanRP != null)
                SetEffectiveRPProperties(zoneItem, ratePlanRP);
        }

        private void SetEffectiveRPProperties(ZoneItem zoneItem, SaleEntityZoneRoutingProduct ratePlanRP)
        {
            zoneItem.EffectiveRoutingProductId = ratePlanRP.RoutingProductId;
            zoneItem.EffectiveRoutingProductName = _rpManager.GetRoutingProductName(ratePlanRP.RoutingProductId);
            zoneItem.EffectiveServiceIds = _rpManager.GetZoneServiceIds(ratePlanRP.RoutingProductId, zoneItem.ZoneId);
        }

        #endregion
    }
}
