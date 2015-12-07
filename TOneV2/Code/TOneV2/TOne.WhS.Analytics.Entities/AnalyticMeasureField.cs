using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum AnalyticMeasureField
    {
        FirstCDRAttempt = 0,
        ABR = 1,
        ASR = 2,
        NER = 3,
        Attempts = 4,
        SuccessfulAttempts = 5,
        FailedAttempts = 6,
        DeliveredAttempts = 7,
        DurationsInSeconds = 8,
        PDDInSeconds = 9,
        UtilizationInSeconds = 10,
        NumberOfCalls = 11,
        DeliveredNumberOfCalls = 12,
        CeiledDuration = 13,
        ACD = 14,
        LastCDRAttempt = 15,
        MaxDurationInSeconds = 16,
        PGAD = 17,
        AveragePDD = 18,
        GreenArea = 19,
        GrayArea = 20,
        DeliveredASR = 21,
        BillingNumberOfCalls = 22,
        CostNets = 23,
        SaleNets = 24,
        Profit = 25,
        CapacityUsageDetails = 26,
        PricedDuration = 27,
        CostRate=28,
        SaleRate=29
    }
}
