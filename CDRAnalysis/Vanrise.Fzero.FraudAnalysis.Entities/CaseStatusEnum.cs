using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{

    public enum CaseStatusEnum
    {
        [Description("Open")]
        Open = 1,

        [Description("Pending")]
        Pending = 2,

        [Description("Closed Fraud")]
        ClosedFraud = 3,

        [Description("Closed White List")]
        ClosedWhiteList = 4,

        [Description("Cancelled")]
        Cancelled = 5
    }
}
