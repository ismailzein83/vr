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

        public DataAnalysisItemParameters Parameters { get; set; }
    }
    public class DataAnalysisItemParameters
    {
        public Dictionary<Guid, DataAnalysisItemParameterSettings> DataAnalysisItemParameterSettingsByItemId { get; set; }
    }

    public class DataAnalysisItemParameterSettings
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public ParameterSettings ParameterSettings { get; set; }
    }

    public class ParameterSettings
    {
        public Dictionary<string, Object> ParameterValues { get; set; }
    }
}