using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum StrategyKind
    {
        [Description("User Defined")]
        UserDefined = 0,

        [Description("System BuiltIn")]
        SystemBuiltIn = 1
    }
   
}
