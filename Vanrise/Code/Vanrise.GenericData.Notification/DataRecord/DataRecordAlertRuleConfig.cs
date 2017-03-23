using System;
using Vanrise.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_DataRecordAlertRuleConfig";

        public string Editor { get; set; }
    }

    public abstract class DataRecordAlertRuleSettings
    {
        public abstract Guid ConfigId { get; }
    }
}