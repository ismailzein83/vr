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
    
    public partial class StrategyThreshold
    {
        public int Id { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<decimal> MaxValue { get; set; }
        public Nullable<int> CriteriaID { get; set; }
    
        public virtual Criteria_Profile Criteria_Profile { get; set; }
        public virtual Strategy Strategy { get; set; }
    }
}
