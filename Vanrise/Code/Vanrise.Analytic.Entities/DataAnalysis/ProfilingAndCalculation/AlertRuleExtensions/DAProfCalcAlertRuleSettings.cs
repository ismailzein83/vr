using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleSettings : VRAlertRuleExtendedSettings
    {
        public Guid OutputItemDefinitionId { get; set; }

        public RecordFilterGroup DataAnalysisFilterGroup { get; set; }

        public DataRecordAlertRuleSettings Settings { get; set; }

        public List<string> GroupingFieldNames { get; set; }

        public TimeSpan MinNotificationInterval { get; set; }

        public DAProfCalcAnalysisPeriod DAProfCalcAnalysisPeriod { get; set; }
    }
}