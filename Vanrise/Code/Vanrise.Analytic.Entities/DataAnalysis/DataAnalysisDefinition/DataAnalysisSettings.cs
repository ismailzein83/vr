using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DataAnalysisSettings : SettingData
    {
        public const string SETTING_TYPE = "VR_Analytic_DataAnalysisSettings";
        public Dictionary<Guid, DataAnalysisItemTab> DataAnalysisItemsSettingsByItemId { get; set; }
    }

    public class DataAnalysisItemTab
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public ParameterSettings ParameterSettings { get; set; }
    }

    public class ParameterSettings
    {
        public Dictionary<string, Object> ParameterValues { get; set; }
    }
}
