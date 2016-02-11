using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum StrategyKindEnum
    {
        [Description("User Defined")]
        UserDefined = 0,

        [Description("System BuiltIn")]
        SystemBuiltIn = 1
    }
   
}
