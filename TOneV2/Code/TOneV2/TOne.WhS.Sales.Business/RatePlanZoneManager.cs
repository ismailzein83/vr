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
	public class RatePlanZoneManager
	{
		public BusinessEntity.Entities.SaleEntityService GetZoneInheritedService(GetZoneInheritedServiceInput input)
		{
			SaleEntityService inheritedService;

			var draftManager = new RatePlanDraftManager();
			draftManager.SaveDraft(input.OwnerType, input.OwnerId, input.NewDraft);

			var ratePlanServiceLocator = new SaleEntityServiceLocator(new RatePlanServiceReadWithCache(input.OwnerType, input.OwnerId, input.EffectiveOn, input.NewDraft));

			if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
			{
				inheritedService = ratePlanServiceLocator.GetSellingProductZoneService(input.OwnerId, input.ZoneId);
			}
			else
			{
				var ratePlanManager = new RatePlanManager();
				int sellingProductId = ratePlanManager.GetSellingProductId(input.OwnerId, input.EffectiveOn, false);
				inheritedService = ratePlanServiceLocator.GetCustomerZoneService(input.OwnerId, sellingProductId, input.ZoneId);
			}

			return inheritedService;
		}
	}
}
