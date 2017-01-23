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
			if (context.SaleZoneIds == null)
				throw new MissingMemberException("SaleZoneIds");

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

			foreach (long zoneId in context.SaleZoneIds)
			{
				var isActionApplicableToZoneInput = new TOne.WhS.Sales.Business.UtilitiesManager.IsActionApplicableToZoneInput()
				{
					OwnerType = context.OwnerType,
					OwnerId = context.OwnerId,
					ZoneId = zoneId,
					BulkAction = context.BulkAction,
					Draft = context.DraftData,
					GetSellingProductZoneRate = getSellingProductZoneRate,
					GetCustomerZoneRate = getCustomerZoneRate
				};

				if (UtilitiesManager.IsActionApplicableToZone(isActionApplicableToZoneInput))
					applicableZoneIds.Add(zoneId);
			}

			return applicableZoneIds;
		}
	}
}
