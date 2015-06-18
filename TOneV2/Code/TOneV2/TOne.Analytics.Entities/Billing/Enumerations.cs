using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public enum TimePeriod 
    {
        Days = 0,
        Weeks = 1,
        Months = 2
    }

    public enum VariationReportOptions 
    {
        InBoundMinutes = 0,
        OutBoundMinutes = 1,
        InOutBoundMinutes = 2,
        TopDestinationMinutes = 3,
        InBoundAmount = 4,
        OutBoundAmount = 5,
        InOutBoundAmount = 6,
        TopDestinationAmount = 7,
        Profit = 8
    }

}

