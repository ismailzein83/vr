using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetStrategies : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        public OutArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            List<Strategy> strategies = new List<Strategy>();
            StrategyManager strategyManager =new StrategyManager();

            foreach (int strategyId in context.GetValue(StrategyIds))
            {
                Strategy strategy = strategyManager.GetStrategy(strategyId);
                if (!strategy.IsEnabled)
                    ContextExtensions.WriteTrackingMessage(context, LogEntryType.Warning, "Strategy named: {0} was not loaded because it is disabled", strategy.Name);
                else
                    strategies.Add(strategy);
            }

            context.SetValue(Strategies, strategies);
        }
    }
}
