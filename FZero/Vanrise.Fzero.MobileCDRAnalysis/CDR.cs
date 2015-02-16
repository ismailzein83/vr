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
    
    public partial class CDR
    {
        public int ID { get; set; }
        public int SourceID { get; set; }
        public string Reference { get; set; }
        public Nullable<System.DateTime> ConnectDateTime { get; set; }
        public Nullable<System.DateTime> DisconnectDateTime { get; set; }
        public Nullable<decimal> DurationInSeconds { get; set; }
        public string IN_TRUNK { get; set; }
        public string OUT_TRUNK { get; set; }
        public string CGPN { get; set; }
        public string CDPN { get; set; }
        public Nullable<int> IGNORE { get; set; }
        public string in_type { get; set; }
        public string out_type { get; set; }
        public string @switch { get; set; }
        public string A_temp { get; set; }
        public string B_temp { get; set; }
        public Nullable<int> IsNormalized { get; set; }
        public Nullable<int> ImportID { get; set; }
    }
}
