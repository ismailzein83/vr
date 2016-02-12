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

    public class UpdateStrategyExecutionProgress : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<StrategyExecutionProgress> StrategyExecutionProgress { get; set; }


        [RequiredArgument]
        public InArgument<ExecuteStrategyForNumberRangeProcessOutput> ExecuteStrategySubProcessEventOutput { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var strategyExecutionProgress = context.GetValue(StrategyExecutionProgress);
            var currentOutput = context.GetValue(ExecuteStrategySubProcessEventOutput);

            strategyExecutionProgress.CDRsProcessed += currentOutput.CDRsProcessed;
            strategyExecutionProgress.NumberOfSubscribers += currentOutput.NumberOfSubscribers;
        }
    }
}
