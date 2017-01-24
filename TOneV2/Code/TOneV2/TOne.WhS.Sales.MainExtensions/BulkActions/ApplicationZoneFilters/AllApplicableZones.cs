using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
	public class AllApplicableZones : BulkActionZoneFilter
	{
		public override Guid ConfigId { get { return new Guid("BDC22FEB-14E1-4F0D-8C3E-EF54A5A36312"); } }

		public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
		{
			if (context.SaleZones == null || context.SaleZones.Count() == 0)
				throw new MissingMemberException("SaleZones");

			var futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());

			Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate = (sellingProductId, zoneId) =>
			{
				return futureRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId);
			};

			Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate = (customerId, sellingProductId, zoneId) =>
			{
				return futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
			};

			List<long> applicableZoneIds = new List<long>();

			foreach (SaleZone saleZone in context.SaleZones)
			{
				var isActionApplicableToZoneInput = new IsActionApplicableToZoneInput()
				{
					OwnerType = context.OwnerType,
					OwnerId = context.OwnerId,
					SaleZone = saleZone,
					BulkAction = context.BulkAction,
					Draft = context.DraftData,
					GetSellingProductZoneRate = getSellingProductZoneRate,
					GetCustomerZoneRate = getCustomerZoneRate
				};

				if (UtilitiesManager.IsActionApplicableToZone(isActionApplicableToZoneInput))
					applicableZoneIds.Add(saleZone.SaleZoneId);
			}

			return applicableZoneIds;
		}
	}
}
