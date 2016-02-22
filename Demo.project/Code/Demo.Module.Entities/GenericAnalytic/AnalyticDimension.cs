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
        Date = 5,
        Hour = 6,
        Currency = 7,
        Day =8,
        Week = 9,
        Month = 10,
        ServiceType = 11 ,
        Direction = 12 ,
        CDRType = 13
    }

    public enum AnalyticSummary
    {
        Sum,
        Max,
        Avg
    }
}
