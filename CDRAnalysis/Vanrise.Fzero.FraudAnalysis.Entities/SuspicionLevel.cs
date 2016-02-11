using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{

    public enum SuspicionLevel
    {
        [Description("Suspicious")]
        Suspicious = 2,

        [Description("Highly Suspicious")]
        HighlySuspicious = 3,

        [Description("Fraud")]
        Fraud = 4
    };

    
}
