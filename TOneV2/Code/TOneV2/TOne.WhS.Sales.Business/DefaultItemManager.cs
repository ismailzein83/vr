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
			var rpManager = new RoutingProductManager();

			SaleEntityZoneRoutingProduct routingProduct;
			SaleEntityZoneRoutingProductSource targetRoutingProductSource;

			if (ownerType == SalePriceListOwnerType.SellingProduct)
			{
				routingProduct = rpLocator.GetSellingProductDefaultRoutingProduct(ownerId);
				targetRoutingProductSource = SaleEntityZoneRoutingProductSource.ProductDefault;
			}
			else
			{
				int sellingProductId = GetSellingProductId(ownerId, effectiveOn, false);
				routingProduct = rpLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId);
				targetRoutingProductSource = SaleEntityZoneRoutingProductSource.CustomerDefault;
			}

			SetCurrentRoutingProductProperties(defaultItem, routingProduct, targetRoutingProductSource, rpManager);
			SetDraft(defaultItem, ownerType, ownerId, rpManager);

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

		private void SetCurrentRoutingProductProperties(DefaultItem defaultItem, SaleEntityZoneRoutingProduct routingProduct, SaleEntityZoneRoutingProductSource targetSource, RoutingProductManager rpManager)
		{
			if (routingProduct == null)
				return;

			defaultItem.CurrentRoutingProductId = routingProduct.RoutingProductId;
			defaultItem.CurrentRoutingProductName = rpManager.GetRoutingProductName(routingProduct.RoutingProductId);
			defaultItem.CurrentRoutingProductBED = routingProduct.BED;
			defaultItem.CurrentRoutingProductEED = routingProduct.EED;
			defaultItem.IsCurrentRoutingProductEditable = routingProduct.Source == targetSource;

			defaultItem.CurrentServiceIds = rpManager.GetDefaultServiceIds(routingProduct.RoutingProductId);
		}

		private void SetDraft(DefaultItem defaultItem, SalePriceListOwnerType ownerType, int ownerId, RoutingProductManager rpManager)
		{
			var draftManager = new RatePlanDraftManager();
			Changes draft = draftManager.GetDraft(ownerType, ownerId);

			if (draft == null || draft.DefaultChanges == null)
				return;

			defaultItem.NewRoutingProduct = draft.DefaultChanges.NewDefaultRoutingProduct;
			defaultItem.ResetRoutingProduct = draft.DefaultChanges.DefaultRoutingProductChange;

			if (defaultItem.NewRoutingProduct != null)
				defaultItem.EffectiveServiceIds = rpManager.GetDefaultServiceIds(defaultItem.NewRoutingProduct.DefaultRoutingProductId);
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
