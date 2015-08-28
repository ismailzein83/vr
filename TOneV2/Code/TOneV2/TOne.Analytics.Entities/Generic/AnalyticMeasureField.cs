using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public enum AnalyticMeasureField
    {
        GroupID = 0,
        Attempts = 1,
        SuccessfulAttempts = 2,
        DurationsInMinutes = 3,
        ASR = 4,
        ACD = 5,
        DeliveredASR = 6,
        AveragePDD = 7,
        NumberOfCalls = 8,
        PricedDuration = 9,
        Sale_Nets = 10,
        Cost_Nets = 11,
        Profit = 12,
        Percentage = 13,
        rownIndex = 14
    }
}
