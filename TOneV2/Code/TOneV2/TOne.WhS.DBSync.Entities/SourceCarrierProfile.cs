using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCarrierProfile : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CompanyName { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string CompanyLogoName { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string BillingContact { get; set; }
        public string BillingEmail { get; set; }
        public string PricingContact { get; set; }
        public string PricingEmail { get; set; }
        public string SupportContact { get; set; }
        public string SupportEmail { get; set; }
        public string RegistrationNumber { get; set; }
        public string AccountManagerEmail { get; set; }
        public string SMSPhoneNumber { get; set; }
        public string Website { get; set; }
        public string BillingDisputeEmail { get; set; }
        public string TechnicalContact { get; set; }
        public string TechnicalEmail { get; set; }
        public string CommercialContact { get; set; }
        public string CommercialEmail { get; set; }
        public string AccountManagerContact { get; set; }
        public string CurrencyId { get; set; }
        public byte DuePeriod { get; set; }
        public bool IsDeleted { get; set; }

    }

    public class SourceCarrierProfileWithAccounts : ISourceItem
    {
        public SourceCarrierProfileWithAccounts()
        {
            this.Customers = new List<SourceCarrierAccount>();
            this.Suppliers = new List<SourceCarrierAccount>();
        }
        public string SourceId
        {
            get;
            set;
        }
        public bool IsDeleted { get; set; }

        #region Financial Props
        public SourcePaymentType CustomerPaymentType { get; set; }
        public SourcePaymentType SupplierPaymentType { get; set; }
        public int? CustomerCreditLimit { get; set; }
        public int? SupplierCreditLimit { get; set; }
        public DateTime? CustomerActivateDate { get; set; }
        public DateTime? CustomerDeactivateDate { get; set; }
        public DateTime? SupplierActivateDate { get; set; }
        public DateTime? SupplierDeactivateDate { get; set; }
        public bool InvoiceByProfile { get; set; }
        public bool IsNettingEnabled { get; set; }

        #endregion

        public List<SourceCarrierAccount> Customers { get; set; }
        public List<SourceCarrierAccount> Suppliers { get; set; }

    }

    public enum SourcePaymentType
    {
        Postpaid = 0,
        Prepaid = 1,
        Undefined = 100,
        Defined_By_Profile = 200
    }
}
