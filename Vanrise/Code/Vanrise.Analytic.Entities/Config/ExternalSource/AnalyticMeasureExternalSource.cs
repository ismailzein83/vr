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

    public class MeasureExternalSourceSetting : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Analytic_MeasureExternalSourceSettings";
        public string Editor { get; set; }
    }
    public class DimensionMappingRuleSetting : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Analytic_DimensionMappingRuleSettings";
        public string Editor { get; set; }
    }
    public class MeasureMappingRuleSetting : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Analytic_MeasureMappingRuleSettings";
        public string Editor { get; set; }
    }
}
