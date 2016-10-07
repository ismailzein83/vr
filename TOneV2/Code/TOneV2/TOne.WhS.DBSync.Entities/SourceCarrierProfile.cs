
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

        public bool IsDeleted { get; set; }
    }
}
