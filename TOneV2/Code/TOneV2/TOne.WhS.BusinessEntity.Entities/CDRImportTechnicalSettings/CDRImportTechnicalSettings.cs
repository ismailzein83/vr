using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CDRImportTechnicalSettings : SettingData
    {
        public CDRImportTechnicalSettingData CdrImportTechnicalSettingData { get; set; }
    }

    public class CDRTechnicalConfiguration
    {
        public Guid RuleDefinitionGuid { get; set; }
    }
}
