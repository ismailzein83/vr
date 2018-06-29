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
        public int GetSerialNumberPartInitialSequence()
        {
            var serialNumberParts = GetAutomatedReportSettings().SerialNumberParts;
            serialNumberParts.ThrowIfNull("serialNumberParts");
            return serialNumberParts.Count;
        }

        public VRAutomatedReportSettings GetAutomatedReportSettings()
        {
            SettingManager settingManager = new SettingManager();
            VRAutomatedReportSettings automatedReportSettings = settingManager.GetSetting<VRAutomatedReportSettings>(VRAutomatedReportSettings.SETTING_TYPE);
            automatedReportSettings.ThrowIfNull("automatedReportSettings");
            return automatedReportSettings;
        }
    }
}
