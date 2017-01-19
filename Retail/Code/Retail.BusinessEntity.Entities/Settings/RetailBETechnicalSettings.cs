using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities.Settings
{
    public class RetailBETechnicalSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_BE_TechnicalSettings";

        public IncludedAccountTypes IncludedAccountTypes { get; set; }
    }

    public class IncludedAccountTypes
    {
        public List<Guid> AcountTypeIds { get; set; }
    }
}
