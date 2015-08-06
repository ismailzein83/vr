using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class LoadNumberProfilesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }


        [RequiredArgument]
        public DateTime ToDate { get; set; }


        [RequiredArgument]
        public List<Strategy> Strategies { get; set; }

    }

    #endregion

    public class LoadNumberProfiles : DependentAsyncActivity<LoadNumberProfilesInput>
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


        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());



            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IPredefinedDataManager predefinedDataManager = FraudDataManagerFactory.GetDataManager<IPredefinedDataManager>();
            IStrategyDataManager strategyManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"]);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Loading CDRs from Database to Memory");

            var aggregateDefinitions = new AggregateManager(inputArgument.Strategies).GetAggregateDefinitions(predefinedDataManager.GetCallClasses());
            string currentSubscriberNumber = null;

            //foreach (var strategy in inputArgument.Strategies)
            //{
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
                                if (currentSubscriberNumber != cdr.MSISDN)
                                {
                                    if (currentSubscriberNumber != null)
                                    {
                                        FinishNumberProfileProcessing(currentSubscriberNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, batchSize, aggregateDefinitions);
                                    }
                                    currentSubscriberNumber = cdr.MSISDN;
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
            if (currentSubscriberNumber != null)
                FinishNumberProfileProcessing(currentSubscriberNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, 0, aggregateDefinitions);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");
            //}

        }

        private void FinishNumberProfileProcessing(string subscriberNumber, ref List<NumberProfile> numberProfileBatch, ref int numberProfilesCount, LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<AggregateDefinition> AggregateDefinitions)
        {
            foreach (var strategy in inputArgument.Strategies)
            {
                NumberProfile numberProfile = new NumberProfile()
                {
                    SubscriberNumber = subscriberNumber,
                    FromDate = inputArgument.FromDate,
                    ToDate = inputArgument.ToDate,
                    StrategyId = strategy.Id
                };
                foreach (var aggregateDef in AggregateDefinitions)
                {
                    numberProfile.AggregateValues.Add(aggregateDef.Name, aggregateDef.Aggregation.GetResult(strategy));
                }
                numberProfileBatch.Add(numberProfile);
            }


            //Console.WriteLine(numberProfileBatch.Count);
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

        protected override LoadNumberProfilesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadNumberProfilesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                Strategies = this.Strategies.Get(context)
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
