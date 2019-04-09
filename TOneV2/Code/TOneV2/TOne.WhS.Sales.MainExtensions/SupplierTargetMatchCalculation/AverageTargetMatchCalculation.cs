using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class AverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("2CB834AD-16BE-46C7-8B26-E65927C2388B"); }
        }
          
        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {

			if (context.RPRouteDetail!=null && context.RPRouteDetail.RouteOptionsDetails != null && context.RPRouteDetail.RouteOptionsDetails.Count() > 0)
			{
				decimal rate = context.RPRouteDetail.RouteOptionsDetails.Average((x) => x.ConvertedSupplierRate);
				context.TargetRates.Add(context.EvaluateRate(rate));
			}
		}
    }
}
