using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleAreaSettingsData : SettingData
    {
        public List<string> FixedKeywords { get; set; }
        public List<string> MobileKeywords { get; set; }

    }
}
