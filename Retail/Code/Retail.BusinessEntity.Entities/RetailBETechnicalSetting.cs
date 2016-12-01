using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RetailBETechnicalSetting : Vanrise.Entities.SettingData
    {
        public AccountGridDefinition GridDefinition { get; set; }

        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }

        public FixedChargingDefinition FixedChargingDefinition { get; set; }
    }
}
