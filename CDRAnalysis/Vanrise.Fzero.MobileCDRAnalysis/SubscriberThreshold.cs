//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscriberThreshold
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DateDay { get; set; }
        public string SubscriberNumber { get; set; }
        public Nullable<decimal> Criteria1 { get; set; }
        public Nullable<decimal> Criteria2 { get; set; }
        public Nullable<decimal> Criteria3 { get; set; }
        public Nullable<decimal> Criteria4 { get; set; }
        public Nullable<decimal> Criteria5 { get; set; }
        public Nullable<decimal> Criteria6 { get; set; }
        public Nullable<decimal> Criteria7 { get; set; }
        public Nullable<decimal> Criteria8 { get; set; }
        public Nullable<decimal> Criteria9 { get; set; }
        public Nullable<decimal> Criteria10 { get; set; }
        public Nullable<decimal> Criteria11 { get; set; }
        public Nullable<decimal> Criteria12 { get; set; }
        public Nullable<decimal> Criteria13 { get; set; }
        public Nullable<decimal> Criteria14 { get; set; }
        public Nullable<decimal> Criteria15 { get; set; }
        public Nullable<decimal> Criteria16 { get; set; }
        public Nullable<int> SuspectionLevelId { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<int> PeriodId { get; set; }
    
        public virtual Period Period { get; set; }
        public virtual Suspicion_Level Suspicion_Level { get; set; }
    }
}
