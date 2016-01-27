﻿using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class CreateStrategiesExecutionItems : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        public OutArgument<List<ExecuteStrategyExecutionItem>> StrategyExecutionItems { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }



        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            List<ExecuteStrategyExecutionItem> strategiesExecutionItems = new List<ExecuteStrategyExecutionItem>();

            StrategyManager strategyManager = new StrategyManager();
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();


            foreach (int strategyID in context.GetValue(StrategyIds))
            {
                Strategy strategy = strategyManager.GetStrategyById(strategyID);
                if (!strategy.IsEnabled)
                    ContextExtensions.WriteTrackingMessage(context, LogEntryType.Warning, "Strategy named: {0} was not loaded because it is disabled", strategy.Name);
                else
                {
                    StrategyExecution strategyExecution = new StrategyExecution() { FromDate = this.FromDate.Get(context), ToDate = this.ToDate.Get(context), PeriodID = strategy.PeriodId, StrategyID = strategyID, ProcessID = ContextExtensions.GetSharedInstanceData(context).InstanceInfo.ProcessInstanceID };

                    strategyExecutionManager.ExecuteStrategy(strategyExecution);

            
                    strategiesExecutionItems.Add(new ExecuteStrategyExecutionItem { StrategyId = strategyID, StrategyExecutionId = strategyExecution.ID });
                }
            }

            
            this.StrategyExecutionItems.Set(context, strategiesExecutionItems);
        }
    }
}
