using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

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

        public bool IncludeWhiteList { get; set; }

        public string NumberPrefix { get; set; }


    }

    public class CreateNumberProfilesOutput
    {
        public long NumberOfSubscribers { get; set; }

    }

    #endregion

    public class CreateNumberProfiles : DependentAsyncActivity<CreateNumberProfilesInput, CreateNumberProfilesOutput>
    {

        #region Arguments

        public InArgument<string> NumberPrefix { get; set; }


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

        [RequiredArgument]
        public InArgument<bool> IncludeWhiteList { get; set; }

        public OutArgument<long> NumberOfSubscribers { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override CreateNumberProfilesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CreateNumberProfilesInput
            {
                NumberPrefix = this.NumberPrefix.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                StrategiesExecutionInfo = this.StrategiesExecutionInfo.Get(context),
                Parameters = this.Parameters.Get(context),
                IncludeWhiteList = this.IncludeWhiteList.Get(context)
            };
        }


        protected override CreateNumberProfilesOutput DoWorkWithResult(CreateNumberProfilesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            HashSet<string> whiteListNumbersHashSet = new HashSet<string>();
            if (!inputArgument.IncludeWhiteList)
                whiteListNumbersHashSet = FillWhiteList(inputArgument, whiteListNumbersHashSet);

            Dictionary<int, NetType> callClassNetTypes = GetCallClassNetTypes();

            List<Strategy> strategies = new List<Strategy>();
            if (inputArgument.StrategiesExecutionInfo != null)
                foreach (var i in inputArgument.StrategiesExecutionInfo)
                {
                    strategies.Add(i.Strategy);
                }

            var aggregateManager = strategies.Count > 0 ?
                new AggregateManager(strategies.ToList<INumberProfileParameters>())
                :
                new AggregateManager(new List<INumberProfileParameters> { inputArgument.Parameters })
                ;
            var aggregateDefinitions = aggregateManager.GetAggregateDefinitions(callClassNetTypes);
            int aggregatesCount = aggregateDefinitions.Count;

            ProcessingNumberProfileItem currentProcessingNumberProfileItem = null;

            int batchSize;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["FraudAnalysis_NumberProfileBatchSize"], out batchSize))
                batchSize = 10000;

            int cdrsCount = 0;
            int cdrIndex = 0;

            int numberProfilesCount = 0;
            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
            long numberOfSubscribers = 0;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrBatch) =>
                        {
                            var cdrs = cdrBatch.CDRs;
                            foreach (var cdr in cdrs)
                            {
                                cdrsCount++;
                                if (whiteListNumbersHashSet.Contains(cdr.MSISDN))
                                        continue;
                                if(currentProcessingNumberProfileItem == null || currentProcessingNumberProfileItem.Number != cdr.MSISDN)
                                {
                                    numberOfSubscribers++;
                                    if(currentProcessingNumberProfileItem != null)
                                    {
                                        FinishNumberProfileProcessing(currentProcessingNumberProfileItem, aggregateDefinitions, aggregatesCount, ref numberProfileBatch, inputArgument);

                                        if (numberProfileBatch.Count >= batchSize)
                                            SendNumberProfileBatch(inputArgument, handle, ref numberProfilesCount, ref numberProfileBatch);
                                    }
                                    currentProcessingNumberProfileItem = new ProcessingNumberProfileItem
                                    {
                                        Number = cdr.MSISDN,
                                        AggregateStates = aggregateManager.CreateAggregateStates(aggregateDefinitions)
                                    };
                                }                               
                                if (cdr.IMEI != null)
                                    currentProcessingNumberProfileItem.IMEIs.Add(cdr.IMEI);

                                for (int i = 0; i < aggregatesCount; i++)
                                {
                                    aggregateDefinitions[i].Aggregation.Evaluate(currentProcessingNumberProfileItem.AggregateStates[i], cdr);
                                }
                            }

                            cdrIndex += cdrs.Count;
                            if (cdrIndex >= 100000)
                            {
                                cdrIndex = 0;
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            if (currentProcessingNumberProfileItem != null)
                FinishNumberProfileProcessing(currentProcessingNumberProfileItem, aggregateDefinitions, aggregatesCount, ref numberProfileBatch, inputArgument);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);

            if (numberProfileBatch.Count > 0)
                SendNumberProfileBatch(inputArgument, handle, ref numberProfilesCount, ref numberProfileBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Profiling CDRs");
            return new CreateNumberProfilesOutput
            {
                NumberOfSubscribers = numberOfSubscribers
            };
        }

        private static HashSet<string> FillWhiteList(CreateNumberProfilesInput inputArgument, HashSet<string> whiteListNumbersHashSet)
        {
            AccountStatusManager accountStatusManager = new AccountStatusManager();
            var whiteListNumbers = accountStatusManager.GetAccountNumbersByNumberPrefixAndStatuses(new List<CaseStatus> { CaseStatus.ClosedWhiteList }, new List<string> { inputArgument.NumberPrefix });
            whiteListNumbersHashSet = new HashSet<string>(whiteListNumbers);
            return whiteListNumbersHashSet;
        }

        private static Dictionary<int, NetType> GetCallClassNetTypes()
        {
            CallClassManager manager = new CallClassManager();
            Dictionary<int, NetType> callClassNetTypes = new Dictionary<int, NetType>();
            var callClasses = manager.GetClasses();
            if (callClasses != null)
            {
                foreach (CallClass callClass in callClasses)
                {
                    callClassNetTypes.Add(callClass.Id, callClass.NetType);
                }
            }
            return callClassNetTypes;
        }

        private void FinishNumberProfileProcessing(ProcessingNumberProfileItem processingNumberProfileItem, List<AggregateDefinition> aggregateDefinitions, int aggregatesCount, ref List<NumberProfile> numberProfileBatch, CreateNumberProfilesInput inputArgument)
        {
            if (inputArgument.StrategiesExecutionInfo != null)
            {
                foreach (var strategyExecutionInfo in inputArgument.StrategiesExecutionInfo)
                {
                    NumberProfile numberProfile = new NumberProfile()
                    {
                        AccountNumber = processingNumberProfileItem.Number,
                        FromDate = inputArgument.FromDate,
                        ToDate = inputArgument.ToDate,
                        StrategyId = strategyExecutionInfo.Strategy.Id,
                        StrategyExecutionID = strategyExecutionInfo.StrategyExecutionId,
                        IMEIs = processingNumberProfileItem.IMEIs
                    };
                    SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, strategyExecutionInfo.Strategy);
                    numberProfileBatch.Add(numberProfile);
                }
            }
            else
            {
                NumberProfile numberProfile = new NumberProfile()
                {
                    AccountNumber = processingNumberProfileItem.Number,
                    FromDate = inputArgument.FromDate,
                    ToDate = inputArgument.ToDate,
                    IMEIs = processingNumberProfileItem.IMEIs
                };
                SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, inputArgument.Parameters);
                numberProfileBatch.Add(numberProfile);
            }

        }

        private static void SetProfileAggregateValues(NumberProfile numberProfile, ProcessingNumberProfileItem processingNumberProfileItem, List<AggregateDefinition> aggregateDefinitions, int aggregatesCount, INumberProfileParameters numberProfileParameters)
        {
            for (int i = 0; i < aggregatesCount; i++)
            {
                var aggregateDef = aggregateDefinitions[i];
                numberProfile.AggregateValues.Add(aggregateDef.KeyName, aggregateDef.Aggregation.GetResult(processingNumberProfileItem.AggregateStates[i], numberProfileParameters));
            }
        }

        private static void SendNumberProfileBatch(CreateNumberProfilesInput inputArgument, AsyncActivityHandle handle, ref int numberProfilesCount, ref List<NumberProfile> numberProfileBatch)
        {
            numberProfilesCount += numberProfileBatch.Count;
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Sent", numberProfilesCount);
            inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
            {
                NumberProfiles = numberProfileBatch
            });
            numberProfileBatch = new List<NumberProfile>();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CreateNumberProfilesOutput result)
        {
            this.NumberOfSubscribers.Set(context, result.NumberOfSubscribers);
        }

        #region Private Classes

        private class ProcessingNumberProfileItem
        {
            public string Number { get; set; }

            HashSet<string> _IMEIs = new HashSet<string>();
            public HashSet<string> IMEIs
            {
                get
                {
                    return _IMEIs;
                }
            }

            public List<AggregateState> AggregateStates { get; set; }
        }

        #endregion

    }
}
