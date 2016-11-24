using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public enum AccountType { Vendor = 1, Customer = 2}
    public enum State { Active = 1, Dormant = 2 , Suspended = 3}

    public class Account
    {
        public int CarrierId { get; set; }
        public AccountType TypeId { get; set; }
        public int AccountId { get; set; }//account_id
         public String FirstName { get; set; }//first_name
        public String LastName { get; set; }//last_name
        public String CompanyName { get; set; }//company_name
        public String ContactDisplay { get; set; }//contact_display
        public String Email { get; set; }//email
        public String WebSite { get; set; }//web_site
        public int BillingCycle {get;set;} //cycle_id
        public int TaxGroupId {get;set;}//tax_group_id
        public int PaymentTerms {get;set;}//pay_terms
        public State CurrentState { get; set; }//state_id   
        public decimal CreditLimit{get;set;} //credit_limit
        public decimal CreditThreshold{get;set;}//threshold_credit
        public decimal CurrentBalance {get;set;}//balance
        public String LogAlias{get;set;}//log_alias
        public String Address{get;set;}//address
        public int PeerVendorId{get; set;}//peer_account_id
        public int ChannelsLimit {get;set;}//channels_limit 
    }
}
