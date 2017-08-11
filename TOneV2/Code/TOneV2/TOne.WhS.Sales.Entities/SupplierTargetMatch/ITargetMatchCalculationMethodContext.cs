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
        RPRouteDetail RPRouteDetail { get; }
        List<SupplierTargetMatchAnalyticOption> Options { set; }
        decimal MarginValue { get; }
        MarginType MarginType { get; }
        SupplierTargetMatchAnalyticOption GetSupplierAnalyticInfo(int supplierId);
        decimal EvaluateRate(decimal originalRate);
    }
}
