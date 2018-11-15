using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class KPISettings : SettingData
    {
        public static string SETTING_TYPE = "VR_Analytic_KPISettings";
        public List<AnalyticTableKPISettings> AnalyticTablesKPISettings { get; set; }
    }
    public class AnalyticTableKPISettings
    {
        public Guid AnalyticTableId { get; set; }
        public List<MeasureStyleRule> MeasureStyleRules { get; set; }
    }
}
