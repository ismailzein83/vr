using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
	public class ApplicableSaleZoneFilter : ISaleZoneFilter
	{
		public BulkActionType ActionType { get; set; }

		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public bool IsExcluded(ISaleZoneFilterContext context)
		{
			if (context.SaleZone == null)
				throw new ArgumentNullException("SaleZone");

			if (context.CustomData == null)
			{
				context.CustomData = (object)new CustomData(this.OwnerType, this.OwnerId);
			}

			CustomData customData = context.CustomData as CustomData;

			var IsActionApplicableToZoneInput = new IsActionApplicableToZoneInput()
			{
				OwnerType = OwnerType,
				OwnerId = OwnerId,
				SaleZone = context.SaleZone,
				BulkAction = ActionType,
				Draft = customData.Draft,
				GetSellingProductZoneRate = customData.GetSellingProductZoneRate,
				GetCustomerZoneRate = customData.GetCustomerZoneRate
			};

			return !UtilitiesManager.IsActionApplicableToZone(IsActionApplicableToZoneInput);
		}

		private class CustomData
		{
			private SaleEntityZoneRateLocator _futureRateLocator;

			public Changes Draft { get; set; }

			public CustomData(SalePriceListOwnerType ownerType, int ownerId)
			{
				_futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
				Draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
			}

			public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId)
			{
				return _futureRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId);
			}

			public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId)
			{
				return _futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
			}
		}
	}
}
