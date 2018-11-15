using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class ConfigManager
    {
        public List<VRAutomatedReportFileNamePart> GetFileNameParts()
        {
            var fileNameParts = GetAutomatedReportSettings().FileNameParts;
            fileNameParts.ThrowIfNull("fileNameParts");
            return fileNameParts;
        }

        public VRAutomatedReportSettings GetAutomatedReportSettings()
        {
            SettingManager settingManager = new SettingManager();
            VRAutomatedReportSettings automatedReportSettings = settingManager.GetSetting<VRAutomatedReportSettings>(VRAutomatedReportSettings.SETTING_TYPE);
            automatedReportSettings.ThrowIfNull("automatedReportSettings");
            return automatedReportSettings;
        }
        public List<MeasureStyleRule> GetAnalytictableKPISettings (Guid analyticTableId)
        {
            SettingManager settingManager = new SettingManager();
            KPISettings kpiSettings = settingManager.GetSetting<KPISettings>(KPISettings.SETTING_TYPE);
            kpiSettings.ThrowIfNull("KPISettings");
            var analyticTableKPISettings = kpiSettings.AnalyticTablesKPISettings.FindRecord(x => x.AnalyticTableId == analyticTableId);
            analyticTableKPISettings.ThrowIfNull("AnalyticTableKPISettings");
            return analyticTableKPISettings.MeasureStyleRules;
        }
    }
}
