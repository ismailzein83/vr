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
    
    public partial class NumberProfile
    {
        public int Id { get; set; }
        public string SubscriberNumber { get; set; }
        public Nullable<System.DateTime> Date_Day { get; set; }
        public Nullable<int> Day_Hour { get; set; }
        public Nullable<int> Count_Out_Calls { get; set; }
        public int Count_In_Calls { get; set; }
        public Nullable<int> Count_Out_Fail { get; set; }
        public int Count_In_Fail { get; set; }
        public Nullable<decimal> Call_Out_Dur_Avg { get; set; }
        public decimal Call_In_Dur_Avg { get; set; }
        public Nullable<decimal> Total_Out_Volume { get; set; }
        public decimal Total_In_Volume { get; set; }
        public Nullable<int> Diff_Output_Numb { get; set; }
        public int Diff_Input_Numbers { get; set; }
        public Nullable<int> Diff_Dest_net { get; set; }
        public Nullable<int> Diff_Sources_net { get; set; }
        public Nullable<int> Total_Cell { get; set; }
        public Nullable<int> count_orig_SMS { get; set; }
        public Nullable<int> count_ter_SMS { get; set; }
        public Nullable<int> Total_IMEI { get; set; }
        public Nullable<int> Count_On_Net { get; set; }
        public Nullable<int> Count_Off_Net { get; set; }
        public Nullable<int> Total_BTS { get; set; }
        public Nullable<int> Count_Out_Inter { get; set; }
        public Nullable<int> Count_In_Inter { get; set; }
    }
}
