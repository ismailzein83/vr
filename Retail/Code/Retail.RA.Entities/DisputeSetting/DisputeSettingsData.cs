using System;
using Vanrise.Entities;

namespace Retail.RA.Entities
{
    public class DisputeSettingsData : SettingData
    {
        public string SerialNumberPattern { get; set; }
        public long InitialSequence { get; set; }
    }
}
