using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{

    public enum AccountStatusSource
    {
        [Description("Case Update")]
        CaseUpdate = 1,

        [Description("Manual Upload")]
        ManualUpload = 2
    }
}
