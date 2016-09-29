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
    public class ZoneServiceManager
    {
        private SaleEntityServiceLocator _serviceLocator;
        private SaleEntityServiceLocator _effectiveServiceLocator;

        public ZoneServiceManager(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, Changes draft)
        {
            _serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(effectiveOn));
            _effectiveServiceLocator = new SaleEntityServiceLocator(new EffectiveSaleEntityServiceReadWithCache(ownerType, ownerId, effectiveOn, draft));
        }

        public void SetSellingProductZoneService(ZoneItem zoneItem, int sellingProductId, ZoneChanges zoneDraft)
        {
            SaleEntityService currentService = _serviceLocator.GetSellingProductZoneService(sellingProductId, zoneItem.ZoneId);

            if (currentService != null)
            {
                zoneItem.CurrentServices = currentService.Services;
                zoneItem.CurrentServiceBED = currentService.BED;
                zoneItem.CurrentServiceEED = currentService.EED;
                zoneItem.IsCurrentServiceEditable = currentService.Source == SaleEntityServiceSource.ProductZone;
            }

            SetDraftZoneService(zoneItem, zoneDraft);
            SetSellingProductEffectiveZoneService(zoneItem, sellingProductId);
        }

        public void SetCustomerZoneService(ZoneItem zoneItem, int customerId, int sellingProductId, ZoneChanges zoneDraft)
        {
            SaleEntityService currentService = _serviceLocator.GetCustomerZoneService(customerId, sellingProductId, zoneItem.ZoneId);

            if (currentService != null)
            {
                zoneItem.CurrentServices = currentService.Services;
                zoneItem.CurrentServiceBED = currentService.BED;
                zoneItem.CurrentServiceEED = currentService.EED;
                zoneItem.IsCurrentServiceEditable = currentService.Source == SaleEntityServiceSource.CustomerZone;
            }

            SetDraftZoneService(zoneItem, zoneDraft);
            SetCustomerEffectiveZoneService(zoneItem, customerId, sellingProductId);
        }

        private void SetDraftZoneService(ZoneItem zoneItem, ZoneChanges zoneDraft)
        {
            if (zoneDraft != null)
            {
                zoneItem.NewService = zoneDraft.NewService;
                zoneItem.ClosedService = zoneDraft.ClosedService;
                zoneItem.ResetService = zoneDraft.ResetService;
            }
        }

        private void SetSellingProductEffectiveZoneService(ZoneItem zoneItem, int sellingProductId)
        {
            SaleEntityService effectiveService = _effectiveServiceLocator.GetSellingProductZoneService(sellingProductId, zoneItem.ZoneId);
            if (effectiveService != null)
                zoneItem.EffectiveServices = effectiveService.Services;
        }

        private void SetCustomerEffectiveZoneService(ZoneItem zoneItem, int customerId, int sellingProductId)
        {
            SaleEntityService effectiveService = _effectiveServiceLocator.GetCustomerZoneService(customerId, sellingProductId, zoneItem.ZoneId);
            if (effectiveService != null)
                zoneItem.EffectiveServices = effectiveService.Services;
        }
    }
}
