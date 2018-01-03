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
    
    public partial class MobileOperator
    {
        public MobileOperator()
        {
            this.EmailCCs = new HashSet<EmailCC>();
            this.RecievedCalls = new HashSet<RecievedCall>();
            this.RelatedNumberMappings = new HashSet<RelatedNumberMapping>();
            this.GeneratedCalls = new HashSet<GeneratedCall>();
        }
    
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Code { get; set; }
        public bool AutoReport { get; set; }
        public bool RepeatedCases { get; set; }
        public bool EnableAutoBlock { get; set; }
        public string AutoBlockEmail { get; set; }
        public bool AutoReportSecurity { get; set; }
        public string AutoReportSecurityEmail { get; set; }
        public Nullable<bool> EnableFTP { get; set; }
        public string FTPAddress { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string FTPPort { get; set; }
        public Nullable<int> FTPType { get; set; }
        public Nullable<bool> IncludeCSVFile { get; set; }
    
        public virtual ICollection<EmailCC> EmailCCs { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<RecievedCall> RecievedCalls { get; set; }
        public virtual ICollection<RelatedNumberMapping> RelatedNumberMappings { get; set; }
        public virtual ICollection<GeneratedCall> GeneratedCalls { get; set; }
    }
}
