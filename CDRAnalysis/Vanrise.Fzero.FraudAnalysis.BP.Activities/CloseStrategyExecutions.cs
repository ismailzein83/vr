using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
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
        public InArgument<long> NumberofSubscribers { get; set; }


        [RequiredArgument]
        public InArgument<long> NumberofCDRs { get; set; }


        [RequiredArgument]
        public InArgument<long> NumberofCases { get; set; }


        [RequiredArgument]
        public InArgument<int> ExecutionDuration { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();

            foreach (ExecuteStrategyExecutionItem item in context.GetValue(ExecuteStrategyExecutionItems))
            {
               strategyExecutionManager.CloseStrategyExecution(item.StrategyExecutionId, context.GetValue( NumberofSubscribers), context.GetValue( NumberofCDRs), context.GetValue( NumberofCases), context.GetValue( ExecutionDuration));
            }
            
        }
    }
}
