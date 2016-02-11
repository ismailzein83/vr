using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetStrategyExecutionStatus : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<long> StrategyExecutionId { get; set; }

        public OutArgument<SuspicionOccuranceStatus> Status { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            StrategyExecutionManager strategyExecutionManager = new StrategyExecutionManager();
            StrategyExecution strategyExecution= strategyExecutionManager.GetStrategyExecution(context.GetValue(StrategyExecutionId));
            context.SetValue(Status, strategyExecution.Status);
        }
    }
}
