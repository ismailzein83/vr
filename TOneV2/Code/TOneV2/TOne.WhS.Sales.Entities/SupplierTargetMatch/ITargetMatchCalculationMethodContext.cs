using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
	public interface ITargetMatchCalculationMethodContext
	{
		RPRouteDetailByZone RPRouteDetail { get; }
		List<decimal> TargetRates { get; }
		decimal EvaluateRate(decimal originalRate);
		SupplierAnalyticDetail SupplierAnalyticDetail { get; set; }
	}
}
