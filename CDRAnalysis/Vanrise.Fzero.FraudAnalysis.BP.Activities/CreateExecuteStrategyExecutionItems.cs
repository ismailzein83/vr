using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class CreateExecuteStrategyExecutionItems : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        public OutArgument<List<ExecuteStrategyExecutionItem>> ExecuteStrategyExecutionItems { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        [RequiredArgument]
        public InArgument<int> UserId { get; set; }



        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            List<ExecuteStrategyExecutionItem> executeStrategiesExecutionItems = new List<ExecuteStrategyExecutionItem>();

            StrategyManager strategyManager = new StrategyManager();
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();


            foreach (int strategyID in context.GetValue(StrategyIds))
            {
                Strategy strategy = strategyManager.GetStrategy(strategyID);
                if (!strategy.Settings.IsEnabled)
                    ContextExtensions.WriteTrackingMessage(context, LogEntryType.Warning, "Strategy named: {0} was not loaded because it is disabled", strategy.Name);
                else
                {
                    StrategyExecution strategyExecution = new StrategyExecution() { FromDate = this.FromDate.Get(context), ToDate = this.ToDate.Get(context), PeriodID = strategy.Settings.PeriodId, StrategyID = strategyID, ProcessID = ContextExtensions.GetSharedInstanceData(context).InstanceInfo.ProcessInstanceID, Status = SuspicionOccuranceStatus.Open, ExecutedBy = this.UserId.Get(context), ExecutionDate = DateTime.Now };

                    strategyExecutionManager.ExecuteStrategy(strategyExecution);

            
                    executeStrategiesExecutionItems.Add(new ExecuteStrategyExecutionItem { StrategyId = strategyID, StrategyExecutionId = strategyExecution.ID });
                }
            }

            
            this.ExecuteStrategyExecutionItems.Set(context, executeStrategiesExecutionItems);
        }
    }
}
