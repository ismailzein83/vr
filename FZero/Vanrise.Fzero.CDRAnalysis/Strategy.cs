//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.CDRAnalysis
{
    using System;
    using System.Collections.Generic;
    
    public partial class Strategy
    {
        public Strategy()
        {
            this.Peak_Time = new HashSet<Peak_Time>();
            this.Related_Criteria = new HashSet<Related_Criteria>();
            this.ReportDetails = new HashSet<ReportDetail>();
            this.Strategy_Min_Values = new HashSet<Strategy_Min_Values>();
            this.Strategy_Suspection_Level = new HashSet<Strategy_Suspection_Level>();
            this.StrategyPeriods = new HashSet<StrategyPeriod>();
            this.StrategyThresholds = new HashSet<StrategyThreshold>();
            this.Subscriber_Values = new HashSet<Subscriber_Values>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    
        public virtual ICollection<Peak_Time> Peak_Time { get; set; }
        public virtual ICollection<Related_Criteria> Related_Criteria { get; set; }
        public virtual ICollection<ReportDetail> ReportDetails { get; set; }
        public virtual ICollection<Strategy_Min_Values> Strategy_Min_Values { get; set; }
        public virtual ICollection<Strategy_Suspection_Level> Strategy_Suspection_Level { get; set; }
        public virtual ICollection<StrategyPeriod> StrategyPeriods { get; set; }
        public virtual ICollection<StrategyThreshold> StrategyThresholds { get; set; }
        public virtual ICollection<Subscriber_Values> Subscriber_Values { get; set; }
    }
}
