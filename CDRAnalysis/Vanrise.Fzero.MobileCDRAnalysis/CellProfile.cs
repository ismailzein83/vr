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
    
    public partial class CellProfile
    {
        public int Id { get; set; }
        public string Cell_Id { get; set; }
        public Nullable<System.DateTime> Date_Day { get; set; }
        public Nullable<int> Day_Hour { get; set; }
        public Nullable<int> Distinct_MSISDN_Calls { get; set; }
        public Nullable<int> Distinct_IMEI { get; set; }
        public Nullable<int> Distinct_MSISDN_Msg { get; set; }
    }
}
