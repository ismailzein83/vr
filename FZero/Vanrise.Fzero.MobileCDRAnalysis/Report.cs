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
    
    public partial class Report
    {
        public Report()
        {
            this.ReportDetails = new HashSet<ReportDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<System.DateTime> ReportDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public string ReportNumber { get; set; }
        public string Description { get; set; }
        public int ReportingStatusID { get; set; }
        public string ReportID { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public Nullable<int> SentBy { get; set; }
    
        public virtual ReportingStatu ReportingStatu { get; set; }
        public virtual ICollection<ReportDetail> ReportDetails { get; set; }
    }
}
