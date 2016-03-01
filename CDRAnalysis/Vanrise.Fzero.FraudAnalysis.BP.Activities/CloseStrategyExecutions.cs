using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class CloseStrategyExecutions : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<ExecuteStrategyExecutionItem>> ExecuteStrategyExecutionItems { get; set; }

        [RequiredArgument]
        public InArgument<StrategyExecutionProgress> StrategyExecutionProgress { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();
            StrategyExecution strategyExecution = new StrategyExecution();
            int executionDuration = 0;
            foreach (ExecuteStrategyExecutionItem item in context.GetValue(ExecuteStrategyExecutionItems))
            {
                strategyExecution = strategyExecutionManager.GetStrategyExecution(item.StrategyExecutionId);
                if (strategyExecution != null)
                    executionDuration = Convert.ToInt32(DateTime.Now.Subtract(strategyExecution.ExecutionDate).TotalSeconds);

                long numberOfSubscribers = context.GetValue(StrategyExecutionProgress).NumberOfSubscribers;
                long cdrsProcessed = context.GetValue(StrategyExecutionProgress).CDRsProcessed;
                Dictionary<int,long> suspicionsPerStrategy = context.GetValue(StrategyExecutionProgress).SuspicionsPerStrategy;
                long suspicionPerCurrentStrategy=(suspicionsPerStrategy !=null && suspicionsPerStrategy.Count>0 ? suspicionsPerStrategy[item.StrategyId] :0);

                strategyExecutionManager.CloseStrategyExecution(item.StrategyExecutionId, numberOfSubscribers, cdrsProcessed, suspicionPerCurrentStrategy, executionDuration);
            }

        }
    }
}
