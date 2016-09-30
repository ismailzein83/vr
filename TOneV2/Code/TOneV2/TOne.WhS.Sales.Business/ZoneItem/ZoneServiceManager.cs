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
        #region Fields / Constructors

        private SaleEntityServiceLocator _serviceLocator;
        private SaleEntityServiceLocator _ratePlanServiceLocator;

        public ZoneServiceManager(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, Changes draft)
        {
            _serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(effectiveOn));
            _ratePlanServiceLocator = new SaleEntityServiceLocator(new RatePlanServiceReadWithCache(ownerType, ownerId, effectiveOn, draft));
        }
        
        #endregion

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

        #region Private Methods

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
            SaleEntityService ratePlanService = _ratePlanServiceLocator.GetSellingProductZoneService(sellingProductId, zoneItem.ZoneId);
            if (ratePlanService != null)
                zoneItem.EffectiveServices = ratePlanService.Services;
        }

        private void SetCustomerEffectiveZoneService(ZoneItem zoneItem, int customerId, int sellingProductId)
        {
            SaleEntityService ratePlanService = _ratePlanServiceLocator.GetCustomerZoneService(customerId, sellingProductId, zoneItem.ZoneId);
            if (ratePlanService != null)
                zoneItem.EffectiveServices = ratePlanService.Services;
        }
        
        #endregion
    }
}
