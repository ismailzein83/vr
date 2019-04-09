using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
	public class WeightedAverageTargetMatchCalculation : TargetMatchCalculationMethod
	{
		public override Guid ConfigId
		{
			get { return new Guid("F8C397CD-1930-4E02-811F-DE2CB203EB85"); }
		}

		public override void Evaluate(ITargetMatchCalculationMethodContext context)
		{
			IEnumerable<RPRouteOptionDetail> routeOptionsDetails = new List<RPRouteOptionDetail>();

			if (context.RPRouteDetail != null && context.RPRouteDetail.RouteOptionsDetails != null && context.RPRouteDetail.RouteOptionsDetails.Count()>0)
			{
				routeOptionsDetails = context.RPRouteDetail.RouteOptionsDetails;
				decimal sumOfDuration = 0;
				decimal sumOfRatesMultipliedByDuration = 0;
				foreach (RPRouteOptionDetail option in routeOptionsDetails)
				{
					SupplierTargetMatchAnalyticItem supplierTargetMatchAnalyticItem;
					if (context.SupplierAnalyticDetail.TryGetValue(option.SupplierId, out supplierTargetMatchAnalyticItem))
					{
						sumOfDuration += supplierTargetMatchAnalyticItem.Duration;
						sumOfRatesMultipliedByDuration += supplierTargetMatchAnalyticItem.Duration * option.ConvertedSupplierRate;
					}
				}
				if (sumOfDuration > 0)
					context.TargetRates.Add(context.EvaluateRate(sumOfRatesMultipliedByDuration / sumOfDuration));
				else
				{
					decimal rate = context.RPRouteDetail.RouteOptionsDetails.Average((x) => x.ConvertedSupplierRate);
					context.TargetRates.Add(context.EvaluateRate(rate));
				}
			}
		}
	}
}
