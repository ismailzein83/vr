using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class LoadNumberProfilesInput
    {
        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }


        [RequiredArgument]
        public DateTime ToDate { get; set; }


        [RequiredArgument]
        public int PeriodId { get; set; }

        [RequiredArgument]
        public List<Strategy> Strategies { get; set; }

    }

    #endregion

    public class LoadNumberProfiles : BaseAsyncActivity<LoadNumberProfilesInput>
    {

        #region Arguments

        [RequiredArgument]
        public  InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        [RequiredArgument]
        public InArgument<int> PeriodId { get; set; }

        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());



            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {
            IStrategyDataManager strategyManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"]);
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");

            foreach (var strategy in inputArgument.Strategies)
            {
                List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
                List<AggregateDefinition> aggregateDefinitions = new AggregateManager(strategy).GetAggregateDefinitions(strategyManager.GetAllCallClasses());

                NumberProfile currentNumberProfile = null;

                dataManager.LoadCDR(inputArgument.FromDate, inputArgument.ToDate, batchSize, (cdr) =>
                {
                    if (currentNumberProfile == null || currentNumberProfile.SubscriberNumber != cdr.MSISDN)
                    {
                        if (currentNumberProfile != null)
                        {
                            FinishNumberProfileProcessing(currentNumberProfile, ref numberProfileBatch, inputArgument, handle, batchSize, aggregateDefinitions);
                        }
                        currentNumberProfile = new NumberProfile()
                        {
                            SubscriberNumber = cdr.MSISDN,
                            FromDate = inputArgument.FromDate,
                            ToDate = inputArgument.ToDate,
                            PeriodId = inputArgument.PeriodId,
                            IsOnNet = 1,
                            StrategyId=strategy.Id
                        };
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

                });

                if (currentNumberProfile != null)
                    FinishNumberProfileProcessing(currentNumberProfile, ref numberProfileBatch, inputArgument, handle, 0, aggregateDefinitions);

                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Ended");
            }

        }

        private void FinishNumberProfileProcessing(NumberProfile currentNumberProfile, ref List<NumberProfile> numberProfileBatch, LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<AggregateDefinition> AggregateDefinitions)
        {
            foreach (var aggregateDef in AggregateDefinitions)
            {
                currentNumberProfile.AggregateValues.Add(aggregateDef.Name, aggregateDef.Aggregation.GetResult());
            }
            numberProfileBatch.Add(currentNumberProfile);
            //Console.WriteLine(numberProfileBatch.Count);
            if (numberProfileBatch.Count >= batchSize)
            {
                inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                {
                    numberProfiles = numberProfileBatch
                });
                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
                numberProfileBatch = new List<NumberProfile>();
            }
        }

        protected override LoadNumberProfilesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadNumberProfilesInput
            {
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                PeriodId = this.PeriodId.Get(context),
                Strategies = this.Strategies.Get(context)
            };
        }

    }
}
