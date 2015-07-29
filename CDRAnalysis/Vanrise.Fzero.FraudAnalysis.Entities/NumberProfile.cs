using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public NumberProfile()
        {
            AggregateValues = new Dictionary<string, decimal>();
        }

        public Dictionary<String, Decimal> AggregateValues { get; set; }
        public String SubscriberNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsOnNet { get; set; }
        public int PeriodId { get; set; }

        public int StrategyId { get; set; } 


         public decimal? CountOutCalls { get; set; } 
         public decimal? DiffOutputNumb { get; set; } 
         public decimal? CountOutInter { get; set; } 
         public decimal? CountInInter { get; set; } 
         public decimal? CallOutDurAvg { get; set; } 
         public decimal? CountOutFail { get; set; } 
         public decimal? CountInFail { get; set; } 
         public decimal? TotalOutVolume { get; set; } 
         public decimal? TotalInVolume { get; set; } 
         public decimal? DiffInputNumbers { get; set; } 
         public decimal? CountOutSMS { get; set; } 
         public decimal? TotalIMEI { get; set; } 
         public decimal? TotalBTS { get; set; } 
         public decimal? TotalDataVolume { get; set; } 
         public decimal? CountInCalls { get; set; } 
         public decimal? CallInDurAvg { get; set; } 
         public decimal? CountOutOnNet { get; set; } 
         public decimal? CountInOnNet { get; set; } 
         public decimal? CountOutOffNet { get; set; } 
         public decimal? CountInOffNet { get; set; } 
         public decimal? CountFailConsecutiveCalls { get; set; } 
         public decimal? CountConsecutiveCalls { get; set; } 
         public decimal? CountInLowDurationCalls { get; set; } 


    }
}
