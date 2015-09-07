using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class CreateNumberProfilesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }
        
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }

        public List<StrategyExecutionInfo> StrategiesExecutionInfo { get; set; }

        public NumberProfileParameters Parameters { get; set; }

    }

    #endregion

    public class CreateNumberProfiles : DependentAsyncActivity<CreateNumberProfilesInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        public InArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }


        public InArgument<NumberProfileParameters> Parameters { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());



            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(CreateNumberProfilesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
                IPredefinedDataManager predefinedDataManager = FraudDataManagerFactory.GetDataManager<IPredefinedDataManager>();
                IStrategyDataManager strategyManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
                INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
                int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"]);
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Loading CDRs from Database to Memory");
                var callClasses = predefinedDataManager.GetCallClasses();
                
            List<Strategy> strategies = new List<Strategy>();
            if(inputArgument.StrategiesExecutionInfo!=null)
                foreach (var i in inputArgument.StrategiesExecutionInfo)
                {
                    strategies.Add(i.Strategy);
                }

                var aggregateDefinitions = strategies.Count >0 ?
                    new AggregateManager(strategies as IEnumerable<INumberProfileParameters>).GetAggregateDefinitions(callClasses)
                    :
                    new AggregateManager(new List<INumberProfileParameters> { inputArgument.Parameters }).GetAggregateDefinitions(callClasses)
                    ;
                string currentAccountNumber = null;

                List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
                int cdrsCount = 0;
                int numberProfilesCount = 0;
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (cdrBatch) =>
                            {
                                var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(cdrBatch.CDRBatchFilePath));
                                System.IO.File.Delete(cdrBatch.CDRBatchFilePath);
                                var cdrs = Vanrise.Common.ProtoBufSerializer.Deserialize<List<CDR>>(serializedCDRs);
                                foreach (var cdr in cdrs)
                                {
                                    if (currentAccountNumber != cdr.MSISDN)
                                    {
                                        if (currentAccountNumber != null)
                                        {
                                            FinishNumberProfileProcessing(currentAccountNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, batchSize, aggregateDefinitions);
                                        }
                                        currentAccountNumber = cdr.MSISDN;
                                        foreach (var aggregateDef in aggregateDefinitions)
                                        {
                                            aggregateDef.Aggregation.Reset();
                                            aggregateDef.Aggregation.EvaluateCDR(cdr);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var aggregateDef in aggregateDefinitions)
                                        {
                                            aggregateDef.Aggregation.EvaluateCDR(cdr);
                                        }
                                    }
                                }
                                cdrsCount += cdrs.Count;
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });
                if (currentAccountNumber != null)
                    FinishNumberProfileProcessing(currentAccountNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, 0, aggregateDefinitions);

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

           
        }

        private void FinishNumberProfileProcessing(string accountNumber, ref List<NumberProfile> numberProfileBatch, ref int numberProfilesCount, CreateNumberProfilesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<AggregateDefinition> AggregateDefinitions)
        {

            if (inputArgument.StrategiesExecutionInfo != null)
            {
                foreach (var strategyExecutionInfo in inputArgument.StrategiesExecutionInfo)
                {
                    NumberProfile numberProfile = new NumberProfile()
                    {
                        AccountNumber = accountNumber,
                        FromDate = inputArgument.FromDate,
                        ToDate = inputArgument.ToDate,
                        StrategyId = strategyExecutionInfo.Strategy.Id,
                        StrategyExecutionID= strategyExecutionInfo.StrategyExecution.ID
                    };
                    foreach (var aggregateDef in AggregateDefinitions)
                    {
                        numberProfile.AggregateValues.Add(aggregateDef.Name, aggregateDef.Aggregation.GetResult(strategyExecutionInfo.Strategy));
                    }
                    numberProfileBatch.Add(numberProfile);
                }
            }
            else
            {
                NumberProfile numberProfile = new NumberProfile()
                {
                    AccountNumber = accountNumber,
                    FromDate = inputArgument.FromDate,
                    ToDate = inputArgument.ToDate
                };
                foreach (var aggregateDef in AggregateDefinitions)
                {
                    numberProfile.AggregateValues.Add(aggregateDef.Name, aggregateDef.Aggregation.GetResult(inputArgument.Parameters));
                }
                numberProfileBatch.Add(numberProfile);
            }


            if (numberProfileBatch.Count >= batchSize)
            {
                numberProfilesCount += numberProfileBatch.Count;
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Sent", numberProfilesCount);
                inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                {
                    NumberProfiles = numberProfileBatch
                });
                numberProfileBatch = new List<NumberProfile>();
            }
        }

        protected override CreateNumberProfilesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CreateNumberProfilesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                StrategiesExecutionInfo = this.StrategiesExecutionInfo.Get(context),
                Parameters = this.Parameters.Get(context)
            };
        }

        private class StrategyNumberProfile
        {
            public Strategy Strategy { get; set; }

            public NumberProfile CurrentNumberProfile { get; set; }

            public List<AggregateDefinition> AggregateDefinitions { get; set; }
        }
    }
}
