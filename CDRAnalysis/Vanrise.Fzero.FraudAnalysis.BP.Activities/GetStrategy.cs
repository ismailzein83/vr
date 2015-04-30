using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    
    public class GetStrategy :  CodeActivity 
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<int> StrategyId { get; set; }

        public OutArgument<Strategy> Strategy { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int strategyId = context.GetValue(StrategyId);
            context.SetValue(Strategy,new StrategyManager().GetStrategy(strategyId));
        }
    }
}
