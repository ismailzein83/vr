using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
	public class LCR2TargetMatchCalculation : TargetMatchCalculationMethod
	{
		public override Guid ConfigId
		{
			get { return new Guid("E393CCB2-E929-4A50-9AE4-AB59565DD69D"); }
		}

		public override void Evaluate(ITargetMatchCalculationMethodContext context)
		{
			if (context.RPRouteDetail != null && context.RPRouteDetail.RouteOptionsDetails != null && context.RPRouteDetail.RouteOptionsDetails.Count() > 0)
			{
				for (int i = 0; i < 2; i++)
				{
					RPRouteOptionDetail lcr = context.RPRouteDetail.RouteOptionsDetails.ElementAtOrDefault(i);
					if (lcr != null)
					{
						context.TargetRates.Add(context.EvaluateRate(lcr.ConvertedSupplierRate));
					}
				}
			}
		}
	}
}
