using System;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CDRImportTechnicalSettings : SettingData
    {
        public CDRTechnicalConfiguration CdrImportTechnicalSettingData { get; set; }
    }

    public class CDRTechnicalConfiguration
    {
        public Guid RuleDefinitionGuid { get; set; }
    }
}
