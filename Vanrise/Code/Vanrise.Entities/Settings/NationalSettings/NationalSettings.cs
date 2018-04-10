using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class NationalSettings : SettingData
    {
        public const string SETTING_TYPE = "VR_Common_NationalSettings";

        public List<int> NationalCountries { get; set; }
    }
}
