using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetStrategiesExecutionInfo : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<int>> StrategyIds { get; set; }

        public OutArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        [RequiredArgument]
        public InArgument<int> PeriodID { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            List<StrategyExecutionInfo> strategiesExecutionInfo = new List<StrategyExecutionInfo>();

            StrategyManager strategyManager = new StrategyManager();

            foreach (int strategyID in context.GetValue(StrategyIds))
            {
                Strategy strategy = strategyManager.GetStrategy(strategyID);
                if (!strategy.IsEnabled)
                    ContextExtensions.WriteTrackingMessage(context, LogEntryType.Warning, "Strategy named: {0} was not loaded because it is disabled", strategy.Name);
                else
                {

                    StrategyExecution strategyExecution= new StrategyExecution();

                    strategyManager.ExecuteStrategy(new StrategyExecution() { FromDate = this.FromDate.Get(context), ToDate = this.ToDate.Get(context), PeriodID = this.PeriodID.Get(context), StrategyID = strategyID, ProcessID = ContextExtensions.GetSharedInstanceData(context).InstanceInfo.ProcessInstanceID });

                    strategiesExecutionInfo.Add(new StrategyExecutionInfo { Strategy = strategy, StrategyExecution = strategyExecution });
                }
            }

            context.SetValue(StrategiesExecutionInfo, strategiesExecutionInfo);
        }
    }
}
