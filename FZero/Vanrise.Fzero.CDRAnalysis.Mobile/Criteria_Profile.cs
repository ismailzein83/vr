//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    using System;
    using System.Collections.Generic;
    
    public partial class Criteria_Profile
    {
        public Criteria_Profile()
        {
            this.Related_Criteria = new HashSet<Related_Criteria>();
            this.Related_Criteria1 = new HashSet<Related_Criteria>();
            this.Strategy_Min_Values = new HashSet<Strategy_Min_Values>();
            this.StrategyPeriods = new HashSet<StrategyPeriod>();
            this.StrategyThresholds = new HashSet<StrategyThreshold>();
            this.Subscriber_Values = new HashSet<Subscriber_Values>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Related_Criteria> Related_Criteria { get; set; }
        public virtual ICollection<Related_Criteria> Related_Criteria1 { get; set; }
        public virtual ICollection<Strategy_Min_Values> Strategy_Min_Values { get; set; }
        public virtual ICollection<StrategyPeriod> StrategyPeriods { get; set; }
        public virtual ICollection<StrategyThreshold> StrategyThresholds { get; set; }
        public virtual ICollection<Subscriber_Values> Subscriber_Values { get; set; }
    }
}
