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
    public class DefaultItemManager
    {
        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var defaultItem = new DefaultItem();

            var rpLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveOn));
            var serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(effectiveOn));

            SaleEntityZoneRoutingProduct routingProduct;
            SaleEntityService service;

            SaleEntityZoneRoutingProductSource targetRoutingProductSource;
            SaleEntityServiceSource targetServiceSource;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                routingProduct = rpLocator.GetSellingProductDefaultRoutingProduct(ownerId);
                service = serviceLocator.GetSellingProductDefaultService(ownerId);

                targetRoutingProductSource = SaleEntityZoneRoutingProductSource.ProductDefault;
                targetServiceSource = SaleEntityServiceSource.ProductDefault;
            }
            else
            {
                int sellingProductId = GetSellingProductId(ownerId, effectiveOn, false);
                routingProduct = rpLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId);
                service = serviceLocator.GetCustomerDefaultService(ownerId, sellingProductId);

                targetRoutingProductSource = SaleEntityZoneRoutingProductSource.CustomerDefault;
                targetServiceSource = SaleEntityServiceSource.CustomerDefault;
            }

            SetCurrentRoutingProductProperties(defaultItem, routingProduct, targetRoutingProductSource);
            SetCurrentServiceProperties(defaultItem, service, targetServiceSource);

            SetDraft(defaultItem, ownerType, ownerId);

            return defaultItem;
        }

        public BusinessEntity.Entities.SaleEntityService GetCustomerDefaultInheritedService(GetCustomerDefaultInheritedServiceInput input)
        {
            var draftManager = new RatePlanDraftManager();
            draftManager.SaveDraft(SalePriceListOwnerType.Customer, input.CustomerId, input.NewDraft);

            var ratePlanServiceLocator = new SaleEntityServiceLocator(new RatePlanServiceReadWithCache(SalePriceListOwnerType.Customer, input.CustomerId, input.EffectiveOn, input.NewDraft));

            var ratePlanManager = new RatePlanManager();
            int sellingProductId = ratePlanManager.GetSellingProductId(input.CustomerId, input.EffectiveOn, false);

            return ratePlanServiceLocator.GetCustomerDefaultService(input.CustomerId, sellingProductId);
        }

        #region Private Methods

        private void SetCurrentRoutingProductProperties(DefaultItem defaultItem, SaleEntityZoneRoutingProduct routingProduct, SaleEntityZoneRoutingProductSource targetSource)
        {
            if (routingProduct != null)
            {
                var rpManager = new RoutingProductManager();
                defaultItem.CurrentRoutingProductId = routingProduct.RoutingProductId;
                defaultItem.CurrentRoutingProductName = rpManager.GetRoutingProductName(routingProduct.RoutingProductId);
                defaultItem.CurrentRoutingProductBED = routingProduct.BED;
                defaultItem.CurrentRoutingProductEED = routingProduct.EED;
                defaultItem.IsCurrentRoutingProductEditable = routingProduct.Source == targetSource;
            }
        }
        private void SetCurrentServiceProperties(DefaultItem defaultItem, SaleEntityService service, SaleEntityServiceSource targetSource)
        {
            if (service != null)
            {
                defaultItem.CurrentServices = service.Services;
                defaultItem.CurrentServiceBED = service.BED;
                defaultItem.CurrentServiceEED = service.EED;
                defaultItem.IsCurrentServiceEditable = service.Source == targetSource;
            }
        }

        private void SetDraft(DefaultItem defaultItem, SalePriceListOwnerType ownerType, int ownerId)
        {
            var draftManager = new RatePlanDraftManager();
            Changes draft = draftManager.GetDraft(ownerType, ownerId);

            if (draft == null || draft.DefaultChanges == null)
                return;

            SetDraftRoutingProduct(defaultItem, draft.DefaultChanges.NewDefaultRoutingProduct, draft.DefaultChanges.DefaultRoutingProductChange);
            SetDraftService(defaultItem, draft.DefaultChanges.NewService, draft.DefaultChanges.ClosedService, draft.DefaultChanges.ResetService);
        }
        private void SetDraftRoutingProduct(DefaultItem defaultItem, DraftNewDefaultRoutingProduct newRoutingProduct, DraftChangedDefaultRoutingProduct changedRoutingProduct)
        {
            defaultItem.NewRoutingProduct = newRoutingProduct;
            defaultItem.ChangedRoutingProduct = changedRoutingProduct;

            // TODO: Remove the code below
            if (newRoutingProduct != null)
            {
                var rpManager = new RoutingProductManager();
                defaultItem.NewRoutingProductId = newRoutingProduct.DefaultRoutingProductId;
                defaultItem.NewRoutingProductName = rpManager.GetRoutingProductName(newRoutingProduct.DefaultRoutingProductId);
                defaultItem.NewRoutingProductBED = newRoutingProduct.BED;
                defaultItem.NewRoutingProductEED = newRoutingProduct.EED;
            }
            else if (changedRoutingProduct != null)
                defaultItem.RoutingProductChangeEED = changedRoutingProduct.EED;
        }
        private void SetDraftService(DefaultItem defaultItem, DraftNewDefaultService newService, DraftClosedDefaultService closedService, DraftResetDefaultService resetService)
        {
            defaultItem.NewService = newService;
            defaultItem.ClosedService = closedService;
            defaultItem.ResetService = resetService;
        }

        private int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerSellingProductManager = new CustomerSellingProductManager();
            int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
            if (!sellingProductId.HasValue)
                throw new NullReferenceException("sellingProductId");
            return sellingProductId.Value;
        }

        #endregion
    }
}
