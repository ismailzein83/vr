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
    
    public partial class vw_ReportedNumber
    {
        public string CLI { get; set; }
        public Nullable<System.DateTime> Date_Day { get; set; }
        public Nullable<decimal> OriginatedVolume { get; set; }
        public int TerminatedVolume { get; set; }
        public Nullable<int> NumberofAttempts { get; set; }
        public Nullable<int> CountDistinctCalledParties { get; set; }
        public int ReportID { get; set; }
        public string MonthYear { get; set; }
    }
}
