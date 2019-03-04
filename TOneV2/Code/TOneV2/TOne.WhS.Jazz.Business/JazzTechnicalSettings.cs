using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Jazz.Business
{
    public class JazzTechnicalSettingData:SettingData
    {
        public const string SETTING_TYPE = "WhS_Jazz_Technical_Settings";
        public Guid AnalyticTableId { get; set; }

    }
}