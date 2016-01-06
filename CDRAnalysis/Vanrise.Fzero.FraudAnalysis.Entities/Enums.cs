using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{

    public enum StrategyKindEnum
    {
        [Description("User Defined")]
        UserDefined = 0,

        [Description("System BuiltIn")]
        SystemBuiltIn = 1
    }

    public enum StrategyStatusEnum
    {
        [Description("Disabled")]
        Disabled = 0,

        [Description("Enabled")]
        Enabled = 1
    }

    public enum PeriodEnum
    {
        [Description("Hourly")]
        Hourly = 1,

        [Description("Daily")]
        Daily = 2
    };

    public enum SuspicionLevel
    {
        [Description("Suspicious")]
        Suspicious = 2,

        [Description("Highly Suspicious")]
        HighlySuspicious = 3,

        [Description("Fraud")]
        Fraud = 4
    };

    public enum SuspicionOccuranceStatus
    {
        [Description("Open")]
        Open = 1,

        [Description("Closed")]
        Closed = 10,

        [Description("Deleted")]
        Deleted = 20
    }

    public enum CaseStatus
    {
        [Description("Open")]
        Open = 1,

        [Description("Pending")]
        Pending = 2,

        [Description("Closed Fraud")]
        ClosedFraud = 3,

        [Description("Closed White List")]
        ClosedWhiteList = 4
    }
}
