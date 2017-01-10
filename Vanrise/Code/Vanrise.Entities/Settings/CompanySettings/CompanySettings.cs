using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CompanySettings : SettingData
    {
        public const string SETTING_TYPE = "VR_Common_CompanySettings";
        public List<CompanySetting> Settings { get; set; }
    }
    public class CompanySetting
    {
        public Guid CompanySettingId { get; set; }
        public string CompanyName { get; set; }
        public string ProfileName { get; set; }
        public long CompanyLogo { get; set; }
        public string RegistrationAddress { get; set; }
        public string RegistrationNumber { get; set; }
        public string VatId { get; set; }
        public bool IsDefault { get; set; }
        public string BillingEmails { get; set; }
        public List<Guid> BankDetails { get; set; }
    }
}
