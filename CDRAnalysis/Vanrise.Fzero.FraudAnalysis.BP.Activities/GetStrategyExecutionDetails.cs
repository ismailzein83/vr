using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;
using System;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class GetStrategyExecutionDetailInput
    {
        public BaseQueue<NumberProfileBatch> InputQueueForNumberProfile { get; set; }

        public BaseQueue<StrategyExecutionDetailBatch> OutputQueueForStrategyExecutionDetail { get; set; }

        public BaseQueue<NumberProfileBatch> OutputQueueForNumberProfile { get; set; }

        public List<StrategyExecutionInfo> StrategiesExecutionInfo { get; set; }
    }

    #endregion


    public class GetStrategyExecutionDetails : DependentAsyncActivity<GetStrategyExecutionDetailInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueueForNumberProfile { get; set; }

        public InOutArgument<BaseQueue<StrategyExecutionDetailBatch>> OutputQueueForStrategyExecutionDetail { get; set; }

        public InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueueForNumberProfile { get; set; }

        [RequiredArgument]
        public InArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueueForStrategyExecutionDetail.Get(context) == null)
                this.OutputQueueForStrategyExecutionDetail.Set(context, new MemoryQueue<StrategyExecutionDetailBatch>());

            if (this.OutputQueueForNumberProfile.Get(context) == null)
                this.OutputQueueForNumberProfile.Set(context, new MemoryQueue<NumberProfileBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GetStrategyExecutionDetailInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Collecting Suspicious Numbers ");

            Dictionary<int, FraudManager> fraudManagers = new Dictionary<int, FraudManager>();
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            List<Strategy> strategies = new List<Strategy>();
            foreach (var strategiesExecutionInfo in inputArgument.StrategiesExecutionInfo)
            {
                strategies.Add(strategiesExecutionInfo.Strategy);
            }

                foreach (var strategy in strategies)
                {
                   fraudManagers.Add(strategy.Id, new FraudManager(strategy));
                }
                int numberProfilesProcessed = 0;
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueueForNumberProfile.TryDequeue(
                            (item) =>
                            {
                                List<NumberProfile> numberProfiles = new List<NumberProfile>();
                                List<StrategyExecutionDetail> strategyExecutionDetails = new List<StrategyExecutionDetail>();

                                foreach (NumberProfile numberProfile in item.NumberProfiles)
                                {
                                    StrategyExecutionDetail strategyExecutionDetail = new StrategyExecutionDetail();

                                    if (fraudManagers[numberProfile.StrategyId].IsNumberSuspicious(numberProfile, out strategyExecutionDetail))
                                    {
                                        strategyExecutionDetails.Add(strategyExecutionDetail);
                                        numberProfiles.Add(numberProfile);
                                    }

                                }
                                if (strategyExecutionDetails.Count > 0)
                                {
                                     inputArgument.OutputQueueForStrategyExecutionDetail.Enqueue(new StrategyExecutionDetailBatch
                                    {
                                        StrategyExecutionDetails = strategyExecutionDetails
                                    });
                                }
                                   
                                if (numberProfiles.Count > 0)
                                    inputArgument.OutputQueueForNumberProfile.Enqueue(new NumberProfileBatch
                                    {
                                        NumberProfiles = numberProfiles
                                    });

                                numberProfilesProcessed += item.NumberProfiles.Count;
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Processed", numberProfilesProcessed);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finshed Collecting Suspicious Numbers ");
        }

        protected override GetStrategyExecutionDetailInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetStrategyExecutionDetailInput()
            {
                InputQueueForNumberProfile = this.InputQueueForNumberProfile.Get(context),
                OutputQueueForStrategyExecutionDetail = this.OutputQueueForStrategyExecutionDetail.Get(context),
                OutputQueueForNumberProfile = this.OutputQueueForNumberProfile.Get(context),
                StrategiesExecutionInfo = this.StrategiesExecutionInfo.Get(context)
            };
        }
    }
}
