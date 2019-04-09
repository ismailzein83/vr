using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR1TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("745B15CA-4649-4780-AD5D-D24C4575D5EB"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
			if (context.RPRouteDetail != null && context.RPRouteDetail.RouteOptionsDetails != null && context.RPRouteDetail.RouteOptionsDetails.Count() > 0)
			{
				RPRouteOptionDetail lcr = context.RPRouteDetail.RouteOptionsDetails.ElementAtOrDefault(0);
				if (lcr != null)
				{
					context.TargetRates.Add(context.EvaluateRate(lcr.ConvertedSupplierRate));
				}
			}
		}
    }
}
