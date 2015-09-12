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

    public class LoadAccountNumbersInput
    {
        public BaseQueue<StrategyExecutionDetailSummaryBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadAccountNumbers : BaseAsyncActivity<LoadAccountNumbersInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<StrategyExecutionDetailSummaryBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<StrategyExecutionDetailSummaryBatch>());

            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["LoadAccountNumbersDirectory"];

        protected override void DoWork(LoadAccountNumbersInput inputArgument, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            int index = 0;
            int totalIndex = 0;
            List<StrategyExecutionDetailSummary> strategyExecutionDetailSummaries = new List<StrategyExecutionDetailSummary>();

            dataManager.LoadAccountNumbersfromStrategyExecutionDetails((strategyExecutionDetailSummary) =>
                {

                    index++;
                    totalIndex++;

                    strategyExecutionDetailSummaries.Add(strategyExecutionDetailSummary);

                   
                    if (index == 1000)
                    {
                        inputArgument.OutputQueue.Enqueue(new StrategyExecutionDetailSummaryBatch() { StrategyExecutionDetailSummaries = strategyExecutionDetailSummaries });
                        Console.WriteLine("{0} Accounts Loaded", totalIndex);
                        index = 0;
                        strategyExecutionDetailSummaries = new List<StrategyExecutionDetailSummary>();
                    }
                     
                    
                    
                });


            if (strategyExecutionDetailSummaries.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new StrategyExecutionDetailSummaryBatch() { StrategyExecutionDetailSummaries = strategyExecutionDetailSummaries });
                strategyExecutionDetailSummaries = new List<StrategyExecutionDetailSummary>();
            }

        }
       

        protected override LoadAccountNumbersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadAccountNumbersInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
