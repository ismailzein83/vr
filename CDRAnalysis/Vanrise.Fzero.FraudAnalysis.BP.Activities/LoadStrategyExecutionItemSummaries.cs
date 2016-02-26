using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
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
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            int index = 0;
            int totalIndex = 0;
            List<StrategyExecutionItemSummary> strategyExecutionItemSummaries = new List<StrategyExecutionItemSummary>();

            dataManager.LoadStrategyExecutionItemSummaries((strategyExecutionItemSummary) =>
                {

                    index++;
                    totalIndex++;

                    strategyExecutionItemSummaries.Add(strategyExecutionItemSummary);

                   
                    if (index == 1000)
                    {
                        inputArgument.OutputQueue.Enqueue(new StrategyExecutionItemSummaryBatch() { StrategyExecutionItemSummaries = strategyExecutionItemSummaries });
                        Console.WriteLine("{0} Accounts Loaded", totalIndex);
                        index = 0;
                        strategyExecutionItemSummaries = new List<StrategyExecutionItemSummary>();
                    }
                     
                    
                    
                });


            if (strategyExecutionItemSummaries.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new StrategyExecutionItemSummaryBatch() { StrategyExecutionItemSummaries = strategyExecutionItemSummaries });
                strategyExecutionItemSummaries = new List<StrategyExecutionItemSummary>();
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
