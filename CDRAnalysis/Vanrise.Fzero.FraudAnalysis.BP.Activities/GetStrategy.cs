using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetStrategy : CodeActivity
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
                if (!s.IsEnabled)
                    ContextExtensions.WriteTrackingMessage(context, LogEntryType.Warning, "Strategy named: {0} was not loaded because it is disabled", s.Name);
                else
                    strategies.Add(s);
            }

            context.SetValue(Strategies, strategies);
        }
    }
}
