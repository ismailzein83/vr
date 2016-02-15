using System.Activities;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public sealed class ConvertExecuteStrategyExecutionItemsToInfos : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<ExecuteStrategyExecutionItem>> ExecuteStrategyExecutionItems { get; set; }

        [RequiredArgument]
        public OutArgument<List<StrategyExecutionInfo>> StrategyExecutionInfos { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            StrategyManager strategyManager = new StrategyManager();
            List<StrategyExecutionInfo> infos = new List<StrategyExecutionInfo>();
            foreach (var item in this.ExecuteStrategyExecutionItems.Get(context))
            {
                StrategyExecutionInfo info = new StrategyExecutionInfo
                {
                    Strategy = strategyManager.GetStrategy(item.StrategyId),
                    StrategyExecutionId = item.StrategyExecutionId
                };
                infos.Add(info);
            }
            this.StrategyExecutionInfos.Set(context, infos);
        }
    }
}
