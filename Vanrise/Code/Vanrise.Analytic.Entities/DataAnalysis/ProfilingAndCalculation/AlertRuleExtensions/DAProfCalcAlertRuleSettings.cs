using System;
using System.Collections.Generic;
using Vanrise.Common.MainExtensions;
using Vanrise.Entities;
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

        /// <summary>
        /// This field has been subtituted by TimePeriod. But we have to keep it for backward compatibility.
        /// </summary>
        public DAProfCalcAnalysisPeriod DAProfCalcAnalysisPeriod { get; set; }

        VRTimePeriod timePeriod;
        public VRTimePeriod TimePeriod
        {
            get
            {
                if (timePeriod == null && DAProfCalcAnalysisPeriod != null)
                {
                    TimeUnit timeUnit;
                    switch (DAProfCalcAnalysisPeriod.AnalysisPeriodTimeUnit)
                    {
                        case DAProfCalcTimeUnit.Days: timeUnit = TimeUnit.Day; break;
                        case DAProfCalcTimeUnit.Hours: timeUnit = TimeUnit.Hour; break;
                        case DAProfCalcTimeUnit.Minutes: timeUnit = TimeUnit.Minute; break;
                        default: throw new NotSupportedException($"Invalid DAProfCalcTimeUnit '{DAProfCalcAnalysisPeriod.AnalysisPeriodTimeUnit}'");
                    }
                    timePeriod = new LastTimePeriod()
                    {
                        StartingFrom = StartingFrom.ExecutionTime,
                        TimeUnit = timeUnit,
                        TimeValue = DAProfCalcAnalysisPeriod.AnalysisPeriodTimeBack
                    };
                }
                return timePeriod;
            }
            set
            {
                timePeriod = value;
            }
        }

        public DAProfCalcAlertRuleFilter DAProfCalcAlertRuleFilter { get; set; }

        public Dictionary<string, Object> ParameterValues { get; set; }
    }
}