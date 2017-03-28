using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

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

        public abstract bool IsRuleMatched(IDataRecordAlertRuleSettingsIsMatchedContext context);
    }

    public interface IDataRecordAlertRuleSettingsIsMatchedContext
    {
        Guid AlertLevelId { set; }

        List<VRAction> Actions { set; }

        Dictionary<string, dynamic> OutputRecords { get; }

        Guid DataRecordTypeId { get; }
    }

    public class DataRecordAlertRuleSettingsIsMatchedContext : IDataRecordAlertRuleSettingsIsMatchedContext
    {
        public Guid AlertLevelId { get; set; }

        public List<VRAction> Actions { get; set; }

        public Dictionary<string, dynamic> OutputRecords { get; set; }

        public Guid DataRecordTypeId { get; set; }
    }
}