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
    
    public partial class ReportDetail
    {
        public int Id { get; set; }
        public Nullable<int> ReportId { get; set; }
        public string SubscriberNumber { get; set; }
        public Nullable<int> StrategyId { get; set; }
    
        public virtual Report Report { get; set; }
        public virtual Report Report1 { get; set; }
        public virtual Strategy Strategy { get; set; }
        public virtual Strategy Strategy1 { get; set; }
    }
}
