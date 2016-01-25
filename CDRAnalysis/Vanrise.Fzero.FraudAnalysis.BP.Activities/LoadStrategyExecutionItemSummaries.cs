using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class LoadStrategyExecutionItemSummariesInput
    {
        public BaseQueue<StrategyExecutionItemSummaryBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadStrategyExecutionItemSummaries : BaseAsyncActivity<LoadStrategyExecutionItemSummariesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<StrategyExecutionItemSummaryBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<StrategyExecutionItemSummaryBatch>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(LoadStrategyExecutionItemSummariesInput inputArgument, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            int index = 0;
            int totalIndex = 0;
            List<StrategyExecutionItemSummary> strategyExecutionDetailSummaries = new List<StrategyExecutionItemSummary>();

            dataManager.LoadStrategyExecutionItemSummaries((strategyExecutionDetailSummary) =>
                {

                    index++;
                    totalIndex++;

                    strategyExecutionDetailSummaries.Add(strategyExecutionDetailSummary);

                   
                    if (index == 1000)
                    {
                        inputArgument.OutputQueue.Enqueue(new StrategyExecutionItemSummaryBatch() { StrategyExecutionItemSummaries = strategyExecutionDetailSummaries });
                        Console.WriteLine("{0} Accounts Loaded", totalIndex);
                        index = 0;
                        strategyExecutionDetailSummaries = new List<StrategyExecutionItemSummary>();
                    }
                     
                    
                    
                });


            if (strategyExecutionDetailSummaries.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new StrategyExecutionItemSummaryBatch() { StrategyExecutionItemSummaries = strategyExecutionDetailSummaries });
                strategyExecutionDetailSummaries = new List<StrategyExecutionItemSummary>();
            }

        }
       

        protected override LoadStrategyExecutionItemSummariesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadStrategyExecutionItemSummariesInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
