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
        //GateWayIn = 3,
        //GateWayOut = 4,
        PortIn = 3,
        PortOut = 4,
        Date = 7,
        Hour = 8,
        Currency = 9,
        Day = 10,
        Week = 11,
        Month = 12,
        ServiceType = 13 ,
        Direction = 14 ,
        CDRType = 15
    }

    public enum AnalyticSummary
    {
        Sum,
        Max,
        Avg
    }
}
