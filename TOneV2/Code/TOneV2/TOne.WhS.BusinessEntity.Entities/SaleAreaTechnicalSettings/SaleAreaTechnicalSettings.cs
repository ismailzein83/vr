using System;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleAreaTechnicalSettings : SettingData
    {
        public SaleAreaTechnicalConfiguration SaleAreaTechnicalSettingData { get; set; }
    }
    public class SaleAreaTechnicalConfiguration
    {
        public Guid TariffRuleDefinitionGuid { get; set; }
    }
}
