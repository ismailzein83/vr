using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum AnalyticDimension
    {
        Customer = 0,
        Supplier = 1,
        Zone = 2,
        Country = 3,
        Switch = 4,
        GateWayIn = 5,
        GateWayOut = 6,
        PortIn = 7,
        PortOut = 8,
        CodeSales = 9,
        CodeBuy = 10,
        Date = 11,
        Hour = 12,
        SupplierZone = 13,
        Currency = 14,
        Day =18,
        Week = 19,
        Month = 20
    }

    public enum AnalyticSummary
    {
        Sum,
        Max,
        Avg
    }
}
