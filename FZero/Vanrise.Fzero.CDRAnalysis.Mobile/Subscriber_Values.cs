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
    
    public partial class Subscriber_Values
    {
        public int Id { get; set; }
        public string SubscriberNumber { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> CriteriaId { get; set; }
        public Nullable<int> StrategyId { get; set; }
        public Nullable<decimal> Value { get; set; }
    
        public virtual Criteria_Profile Criteria_Profile { get; set; }
        public virtual Strategy Strategy { get; set; }
    }
}
