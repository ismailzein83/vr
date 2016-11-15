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
        public string CompanyName { get; set; }
        public long CompanyLogo { get; set; }
        public string RegistrationAddress { get; set; }
        public string RegistrationNumber { get; set; }
        public string VatId { get; set; }
        public bool IsDefault { get; set; }
    }
}
