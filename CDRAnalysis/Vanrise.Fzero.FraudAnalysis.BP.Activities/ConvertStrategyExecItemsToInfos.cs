using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public sealed class ConvertStrategyExecItemsToInfos : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<ExecuteStrategyExecutionItem>> StrategyExecutionItems { get; set; }

        [RequiredArgument]
        public OutArgument<List<StrategyExecutionInfo>> StrategyExecutionInfos { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            StrategyManager strategyManager = new StrategyManager();
            List<StrategyExecutionInfo> infos = new List<StrategyExecutionInfo>();
            foreach(var item in this.StrategyExecutionItems.Get(context))
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
