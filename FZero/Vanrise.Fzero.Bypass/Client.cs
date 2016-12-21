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
    
    public partial class Client
    {
        public Client()
        {
            this.EmailCCs = new HashSet<EmailCC>();
            this.RecievedCalls = new HashSet<RecievedCall>();
            this.Users = new HashSet<User>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> SendDailyReport { get; set; }
        public Nullable<bool> SendWeeklyReport { get; set; }
        public Nullable<bool> SendMonthlyReport { get; set; }
        public Nullable<bool> ClientReport { get; set; }
        public string ClientEmail { get; set; }
        public Nullable<int> GMT { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> PrefixLength { get; set; }
        public string FraudPrefix { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<bool> ClientReportSecurity { get; set; }
    
        public virtual ICollection<EmailCC> EmailCCs { get; set; }
        public virtual ICollection<RecievedCall> RecievedCalls { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
