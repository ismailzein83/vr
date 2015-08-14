using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierMask
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public int CountryId { get; set; }
        public string RegistrationNumber { get; set; }
        public string VatID { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telephone3 { get; set; }
        public string Fax1 { get; set; }
        public string Fax2 { get; set; }
        public string Fax3 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public long CompanyLogo { get; set; }
        public bool IsBankReferences { get; set; }
        public string BillingContact { get; set; }
        public string BillingEmail { get; set; }
        public string PricingContact { get; set; }
        public string PricingEmail { get; set; }
        public string AccountManagerEmail { get; set; }
        public string SupportContact { get; set; }
        public string SupportEmail { get; set; }
        public string CurrencyId { get; set; }
        public string PriceList { get; set; }
        public string MaskInvoiceformat { get; set; }
        public int MaskOverAllCounter { get; set; }
        public int YearlyMaskOverAllCounter { get; set; }

    }
}
