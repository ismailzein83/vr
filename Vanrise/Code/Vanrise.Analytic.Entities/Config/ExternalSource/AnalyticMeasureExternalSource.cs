using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureExternalSource
    {
        public Guid AnalyticMeasureExternalSourceConfigId { get; set; }
        public string Name { get; set; }
        public AnalyticMeasureExternalSourceConfig Config { get; set; }
    }

    public class MeasureExternalSourceSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_MeasureExternalSourceSettings";
        public string Editor { get; set; }
    }
    public class DimensionMappingRuleSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_DimensionMappingRuleSettings";
        public string Editor { get; set; }
    }
    public class MeasureMappingRuleSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_MeasureMappingRuleSettings";
        public string Editor { get; set; }
    }
}
