using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TestCallAnalysis.Entities
{
    public class TCAnalSettingsData : SettingData
    {
        public static string SETTING_TYPE = "Retail_TCAnal_InstanceSettings";
        public TimeSpan TimeMargin { get; set; }
        public TimeSpan TimeOutMargin { get; set; }
    }
}
