using System;
using System.Collections.Generic;

namespace TABS.Extensibility
{
    /// <summary>
    /// An Alert Criteria. Processes given traffic and generates alerts according to the "Criteria", 
    /// current "State" and "Alert Actions".
    /// </summary>
    public interface IAlertCriteria
    {
        TimeSpan? AlertingTimeSpan { get; set; }
        //int? AlertingRunCount { get; set; }
        //int RunCount { get; set; }
        bool IsEnabled { get; set; }
        DateTime? LastChecked { get; set; }
        DateTime? FirstCheckeded { get; set; }
        AlertLevel AlertLevel { get; set; }
        string Tag { get; set; }
        string FiltersSummary { get; }
        string StateSummary { get; }
        string ThresholdsSummary { get; }
        string Source { get; set; }

        IEnumerable<IAlert> ProcessTraffic(IEnumerable<Billing_CDR_Base> cdrs);
        IEnumerable<IAlert> GetTimeAlerts();
        IEnumerable<IAlert> ProcessTrafficStats(IEnumerable<TrafficStats> tStats);

        void ResetState();
        //For maintenance purposes
        void Clean();
    }
}
