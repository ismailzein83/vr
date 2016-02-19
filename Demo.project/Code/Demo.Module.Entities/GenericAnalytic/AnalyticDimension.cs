using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum AnalyticDimension
    {
        Operator = 0,
        Zone = 1,
        Country = 2,
        GateWayIn = 3,
        GateWayOut = 4,
        PortIn = 7,
        PortOut = 5,
        Date = 6,
        Hour = 7,
        Currency = 8,
        Day =9,
        Week = 10,
        Month = 11,
        ServiceType = 12 ,
        Direction = 13 ,
        CDRType = 14
    }

    public enum AnalyticSummary
    {
        Sum,
        Max,
        Avg
    }
}
