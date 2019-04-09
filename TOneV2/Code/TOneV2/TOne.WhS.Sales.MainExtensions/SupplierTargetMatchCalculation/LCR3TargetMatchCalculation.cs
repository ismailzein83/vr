using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR3TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("96EA70AD-3F21-4B55-BC5E-70ACE0469710"); } }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
			if (context.RPRouteDetail != null && context.RPRouteDetail.RouteOptionsDetails != null && context.RPRouteDetail.RouteOptionsDetails.Count() > 0)
			{
				for (int i = 0; i < 3; i++)
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
