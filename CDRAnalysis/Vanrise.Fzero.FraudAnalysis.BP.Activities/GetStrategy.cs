using System.Activities;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    
    public class GetStrategy :  CodeActivity 
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        public OutArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            List<Strategy> strategies = new List<Strategy>();

            foreach (int strategyId in context.GetValue(StrategyIds))
            {
                Strategy s = new StrategyManager().GetStrategy(strategyId);
                strategies.Add(s);
            }

            context.SetValue(Strategies, strategies);
        }
    }
}
