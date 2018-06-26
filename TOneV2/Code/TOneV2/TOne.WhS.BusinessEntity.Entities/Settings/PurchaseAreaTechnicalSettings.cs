using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PurchaseAreaTechnicalSettings : SettingData
    {
        public PurchaseAreaTechnicalSettingsData PurchaseAreaSettingsData { get; set; }
    }
    public class PurchaseAreaTechnicalSettingsData
    {
        public bool IsAutoImportActivated { get; set; }
    }
    
}
