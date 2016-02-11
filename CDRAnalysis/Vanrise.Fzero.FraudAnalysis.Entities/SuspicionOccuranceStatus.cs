using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum SuspicionOccuranceStatus
    {
        [Description("Open")]
        Open = 1,

        [Description("Closed")]
        Closed = 10,

        [Description("Cancelled")]
        Cancelled = 20
    }
}
