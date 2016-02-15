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

                strategyExecutionManager.CloseStrategyExecution(item.StrategyExecutionId, context.GetValue(StrategyExecutionProgress).NumberOfSubscribers, context.GetValue(StrategyExecutionProgress).CDRsProcessed, context.GetValue(StrategyExecutionProgress).SuspicionsPerStrategy[item.StrategyId], executionDuration);
            }

        }
    }
}
