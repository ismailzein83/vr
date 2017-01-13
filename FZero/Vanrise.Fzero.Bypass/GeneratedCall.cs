//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.Bypass
{
    using System;
    using System.Collections.Generic;
    
    public partial class GeneratedCall
    {
        public int ID { get; set; }
        public int SourceID { get; set; }
        public Nullable<int> MobileOperatorID { get; set; }
        public int StatusID { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public int ReportingStatusID { get; set; }
        public int DurationInSeconds { get; set; }
        public Nullable<int> MobileOperatorFeedbackID { get; set; }
        public string a_number { get; set; }
        public string b_number { get; set; }
        public string CLI { get; set; }
        public string OriginationNetwork { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public Nullable<int> ReportID { get; set; }
        public System.DateTime AttemptDateTime { get; set; }
        public Nullable<System.DateTime> LevelOneComparisonDateTime { get; set; }
        public Nullable<System.DateTime> LevelTwoComparisonDateTime { get; set; }
        public Nullable<System.DateTime> FeedbackDateTime { get; set; }
        public Nullable<System.DateTime> AssignmentDateTime { get; set; }
        public Nullable<int> ImportID { get; set; }
        public Nullable<int> ReportingStatusChangedBy { get; set; }
        public Nullable<bool> Level1Comparison { get; set; }
        public Nullable<bool> Level2Comparison { get; set; }
        public Nullable<int> ToneFeedbackID { get; set; }
        public string FeedbackNotes { get; set; }
        public string Carrier { get; set; }
        public string Reference { get; set; }
        public string Type { get; set; }
        public Nullable<int> ReportingStatusSecurityID { get; set; }
        public Nullable<int> ReportSecID { get; set; }
    
        public virtual MobileOperator MobileOperator { get; set; }
    }
}
