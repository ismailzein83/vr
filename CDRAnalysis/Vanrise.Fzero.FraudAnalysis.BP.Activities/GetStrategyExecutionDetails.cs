﻿using System.Activities;
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

    public class GetStrategyExecutionItemInput
    {
        public BaseQueue<NumberProfileBatch> InputQueueForNumberProfile { get; set; }

        public BaseQueue<StrategyExecutionItemBatch> OutputQueueForStrategyExecutionItem { get; set; }

        public List<StrategyExecutionInfo> StrategiesExecutionInfo { get; set; }
    }

    public class GetStrategyExecutionItemOuput
    {
        public long NumberOfSuspicions { get; set; }
    }

    #endregion


    public class GetStrategyExecutionItem : DependentAsyncActivity<GetStrategyExecutionItemInput, GetStrategyExecutionItemOuput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueueForNumberProfile { get; set; }

        public InOutArgument<BaseQueue<StrategyExecutionItemBatch>> OutputQueueForStrategyExecutionItem { get; set; }

        [RequiredArgument]
        public InArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }

        public OutArgument<long> NumberOfSuspicions { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueueForStrategyExecutionItem.Get(context) == null)
                this.OutputQueueForStrategyExecutionItem.Set(context, new MemoryQueue<StrategyExecutionItemBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override GetStrategyExecutionItemInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetStrategyExecutionItemInput()
            {
                InputQueueForNumberProfile = this.InputQueueForNumberProfile.Get(context),
                OutputQueueForStrategyExecutionItem = this.OutputQueueForStrategyExecutionItem.Get(context),
                StrategiesExecutionInfo = this.StrategiesExecutionInfo.Get(context)
            };
        }

        protected override GetStrategyExecutionItemOuput DoWorkWithResult(GetStrategyExecutionItemInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
            long numberOfSuspicions = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueueForNumberProfile.TryDequeue(
                        (item) =>
                        {
                            List<NumberProfile> numberProfiles = new List<NumberProfile>();
                            List<StrategyExecutionItem> strategyExecutionDetails = new List<StrategyExecutionItem>();

                            foreach (NumberProfile numberProfile in item.NumberProfiles)
                            {
                                StrategyExecutionItem strategyExecutionDetail = new StrategyExecutionItem();

                                if (fraudManagers[numberProfile.StrategyId].IsNumberSuspicious(numberProfile, out strategyExecutionDetail))
                                {
                                    numberOfSuspicions++;
                                    strategyExecutionDetails.Add(strategyExecutionDetail);
                                }

                            }

                            if (strategyExecutionDetails.Count > 0)
                                inputArgument.OutputQueueForStrategyExecutionItem.Enqueue(new StrategyExecutionItemBatch
                                {
                                    StrategyExecutionItem = strategyExecutionDetails
                                });

                            numberProfilesProcessed += item.NumberProfiles.Count;
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Processed", numberProfilesProcessed);

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finshed Collecting Suspicious Numbers ");
            return new GetStrategyExecutionItemOuput
            {
                NumberOfSuspicions = numberOfSuspicions
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetStrategyExecutionItemOuput result)
        {
            this.NumberOfSuspicions.Set(context, result.NumberOfSuspicions);
        }
    }
}
