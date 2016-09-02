using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
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
                                if (currentProcessingNumberProfileItem == null || currentProcessingNumberProfileItem.Number != cdr.MSISDN)
                                {
                                    numberOfSubscribers++;
                                    if (currentProcessingNumberProfileItem != null)
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








//using System;
//using System.Activities;
//using System.Collections.Generic;
//using System.Linq;
//using Vanrise.BusinessProcess;
//using Vanrise.Common;
//using Vanrise.Common.Business;
//using Vanrise.Entities;
//using Vanrise.Fzero.CDRImport.Entities;
//using Vanrise.Fzero.FraudAnalysis.Aggregates;
//using Vanrise.Fzero.FraudAnalysis.BP.Arguments;
//using Vanrise.Fzero.FraudAnalysis.Business;
//using Vanrise.Fzero.FraudAnalysis.Data;
//using Vanrise.Fzero.FraudAnalysis.Entities;
//using Vanrise.Queueing;

//namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
//{

//    #region Arguments Classes

//    public class CreateNumberProfilesInput
//    {
//        public BaseQueue<CDRBatch> InputQueue { get; set; }

//        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

//        public DateTime FromDate { get; set; }

//        public DateTime ToDate { get; set; }

//        public DateTime CDRRangeFromDate { get; set; }

//        public List<StrategyExecutionInfo> StrategiesExecutionInfo { get; set; }

//        public NumberProfileParameters Parameters { get; set; }

//        public bool IncludeWhiteList { get; set; }

//        public string NumberPrefix { get; set; }


//    }

//    public class CreateNumberProfilesOutput
//    {
//        public long NumberOfSubscribers { get; set; }

//    }

//    #endregion

//    public class CreateNumberProfiles : DependentAsyncActivity<CreateNumberProfilesInput, CreateNumberProfilesOutput>
//    {

//        #region Arguments

//        public InArgument<string> NumberPrefix { get; set; }


//        [RequiredArgument]
//        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

//        [RequiredArgument]
//        public InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

//        [RequiredArgument]
//        public InArgument<DateTime> FromDate { get; set; }


//        [RequiredArgument]
//        public InArgument<DateTime> ToDate { get; set; }

//        public InArgument<DateTime> CDRRangeFromDate { get; set; }

//        public InArgument<List<StrategyExecutionInfo>> StrategiesExecutionInfo { get; set; }
        
//        public InArgument<NumberProfileParameters> Parameters { get; set; }

//        [RequiredArgument]
//        public InArgument<bool> IncludeWhiteList { get; set; }

//        public OutArgument<long> NumberOfSubscribers { get; set; }

//        #endregion

//        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
//        {
//            if (this.OutputQueue.Get(context) == null)
//                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());

//            base.OnBeforeExecute(context, handle);
//        }

//        protected override CreateNumberProfilesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
//        {
//            return new CreateNumberProfilesInput
//            {
//                NumberPrefix = this.NumberPrefix.Get(context),
//                InputQueue = this.InputQueue.Get(context),
//                OutputQueue = this.OutputQueue.Get(context),
//                FromDate = this.FromDate.Get(context),
//                ToDate = this.ToDate.Get(context),
//                CDRRangeFromDate = this.CDRRangeFromDate.Get(context),
//                StrategiesExecutionInfo = this.StrategiesExecutionInfo.Get(context),
//                Parameters = this.Parameters.Get(context),
//                IncludeWhiteList = this.IncludeWhiteList.Get(context)
//            };
//        }


//        protected override CreateNumberProfilesOutput DoWorkWithResult(CreateNumberProfilesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
//        {
//            HashSet<string> whiteListNumbersHashSet = new HashSet<string>(); ;
//            if (!inputArgument.IncludeWhiteList)
//                whiteListNumbersHashSet = FillWhiteList(inputArgument, whiteListNumbersHashSet);

//            Dictionary<int, NetType> callClassNetTypes = GetCallClassNetTypes();

//            List<Strategy> strategies = new List<Strategy>();
//            if (inputArgument.StrategiesExecutionInfo != null)
//                foreach (var i in inputArgument.StrategiesExecutionInfo)
//                {
//                    strategies.Add(i.Strategy);
//                }
            
//            var aggregateManager = strategies.Count > 0 ?
//                new AggregateManager(strategies.ToList<INumberProfileParameters>())
//                :
//                new AggregateManager(new List<INumberProfileParameters> { inputArgument.Parameters })
//                ;
//            var aggregateDefinitions = aggregateManager.GetAggregateDefinitions(callClassNetTypes);
//            int aggregatesCount = aggregateDefinitions.Count;

//            Dictionary<string, ProcessingNumberProfileItem> processingNumberProfilesByMSISDN = new Dictionary<string, ProcessingNumberProfileItem>();

//            int batchSize;
//            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["FraudAnalysis_NumberProfileBatchSize"], out batchSize))
//                batchSize = 1000;

//            int cdrsCount = 0;
//            int cdrIndex = 0;

//            int numberProfilesCount = 0;
//            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
//            long numberOfSubscribers = 0;

//            NumberProfileGroupingHandler groupingHandler = new NumberProfileGroupingHandler
//            {
//                FromDate = inputArgument.FromDate,
//                ToDate = inputArgument.ToDate,
//                NumberProfileProcessor = new NumberProfileProcessorForStrategies { StrategyIds = strategies.Select(itm => itm.Id).ToList() },
//                ExecuteStrategiesExecutionItems = inputArgument.StrategiesExecutionInfo.Select(itm => new ExecuteStrategyExecutionItem { StrategyId = itm.Strategy.Id, StrategyExecutionId = itm.StrategyExecutionId }).ToList(),
//                Parameters = inputArgument.Parameters
//            };

//            string dataGroupingName = String.Format("CDRAnalysis_CreateNumberProfile_{0}_{1}", handle.SharedInstanceData.InstanceInfo.ParentProcessID.HasValue ? handle.SharedInstanceData.InstanceInfo.ParentProcessID.Value : handle.SharedInstanceData.InstanceInfo.ProcessInstanceID, inputArgument.CDRRangeFromDate);
//            DistributedDataGrouper dataGrouper = new DistributedDataGrouper(dataGroupingName, groupingHandler);

//            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
//            {
//                bool hasItem = false;
//                do
//                {
//                    hasItem = inputArgument.InputQueue.TryDequeue(
//                        (cdrBatch) =>
//                        {
//                            var cdrs = cdrBatch.CDRs;
//                            foreach (var cdr in cdrs)
//                            {
//                                cdrsCount++;
//                                if (whiteListNumbersHashSet.Contains(cdr.MSISDN))
//                                        continue;
//                                var currentProcessingNumberProfileItem = processingNumberProfilesByMSISDN.GetOrCreateItem(cdr.MSISDN,
//                                    () =>
//                                    {
//                                        return new ProcessingNumberProfileItem
//                                        {
//                                            Number = cdr.MSISDN,
//                                            AggregateStates = aggregateManager.CreateAggregateStates(aggregateDefinitions),
//                                            IMEIs = new HashSet<string>()
//                                        };
//                                    });
//                                if(currentProcessingNumberProfileItem == null || currentProcessingNumberProfileItem.Number != cdr.MSISDN)
//                                {
//                                    numberOfSubscribers++;
//                                    if(currentProcessingNumberProfileItem != null)
//                                    {
//                                        FinishNumberProfileProcessing(currentProcessingNumberProfileItem, aggregateDefinitions, aggregatesCount, ref numberProfileBatch, inputArgument);

//                                        if (numberProfileBatch.Count >= batchSize)
//                                            SendNumberProfileBatch(inputArgument, handle, ref numberProfilesCount, ref numberProfileBatch);
//                                    }
//                                    currentProcessingNumberProfileItem = new ProcessingNumberProfileItem
//                                    {
//                                        Number = cdr.MSISDN,
//                                        AggregateStates = aggregateManager.CreateAggregateStates(aggregateDefinitions),
//                                        IMEIs = new HashSet<string>()
//                                    };
//                                }                               
//                                if (cdr.IMEI != null)
//                                    currentProcessingNumberProfileItem.IMEIs.Add(cdr.IMEI);

//                                for (int i = 0; i < aggregatesCount; i++)
//                                {
//                                    aggregateDefinitions[i].Aggregation.Evaluate(currentProcessingNumberProfileItem.AggregateStates[i], cdr);
//                                }
//                            }

//                            if(processingNumberProfilesByMSISDN.Count >= batchSize)
//                            {
//                                dataGrouper.DistributeGroupingItems(processingNumberProfilesByMSISDN.Values.Cast<IDataGroupingItem>().ToList());
//                                numberOfSubscribers += processingNumberProfilesByMSISDN.Count;
//                                processingNumberProfilesByMSISDN.Clear();
//                            }

//                            cdrIndex += cdrs.Count;
//                            if (cdrIndex >= 100000)
//                            {
//                                cdrIndex = 0;
//                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);
//                            }
//                        });
//                }
//                while (!ShouldStop(handle) && hasItem);
//            });
//            if (processingNumberProfilesByMSISDN.Count >= 0)
//            {
//                dataGrouper.DistributeGroupingItems(processingNumberProfilesByMSISDN.Values.Cast<IDataGroupingItem>().ToList());
//                numberOfSubscribers += processingNumberProfilesByMSISDN.Count;
//                processingNumberProfilesByMSISDN.Clear();
//            }

//            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);

//            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Profiling CDRs");
//            return new CreateNumberProfilesOutput
//            {
//                NumberOfSubscribers = numberOfSubscribers
//            };
//        }

//        private static HashSet<string> FillWhiteList(CreateNumberProfilesInput inputArgument, HashSet<string> whiteListNumbersHashSet)
//        {
//            AccountStatusManager accountStatusManager = new AccountStatusManager();
//            var whiteListNumbers = accountStatusManager.GetAccountNumbersByNumberPrefixAndStatuses(new List<CaseStatus> { CaseStatus.ClosedWhiteList }, new List<string> { inputArgument.NumberPrefix });
//            whiteListNumbersHashSet = new HashSet<string>(whiteListNumbers);
//            return whiteListNumbersHashSet;
//        }

//        internal static Dictionary<int, NetType> GetCallClassNetTypes()
//        {
//            CallClassManager manager = new CallClassManager();
//            Dictionary<int, NetType> callClassNetTypes = new Dictionary<int, NetType>();
//            var callClasses = manager.GetClasses();
//            if (callClasses != null)
//            {
//                foreach (CallClass callClass in callClasses)
//                {
//                    callClassNetTypes.Add(callClass.Id, callClass.NetType);
//                }
//            }
//            return callClassNetTypes;
//        }

//        private void FinishNumberProfileProcessing(ProcessingNumberProfileItem processingNumberProfileItem, List<AggregateDefinition> aggregateDefinitions, int aggregatesCount, ref List<NumberProfile> numberProfileBatch, CreateNumberProfilesInput inputArgument)
//        {
//            if (inputArgument.StrategiesExecutionInfo != null)
//            {
//                foreach (var strategyExecutionInfo in inputArgument.StrategiesExecutionInfo)
//                {
//                    NumberProfile numberProfile = new NumberProfile()
//                    {
//                        AccountNumber = processingNumberProfileItem.Number,
//                        FromDate = inputArgument.FromDate,
//                        ToDate = inputArgument.ToDate,
//                        StrategyId = strategyExecutionInfo.Strategy.Id,
//                        StrategyExecutionID = strategyExecutionInfo.StrategyExecutionId,
//                        IMEIs = processingNumberProfileItem.IMEIs
//                    };
//                    SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, strategyExecutionInfo.Strategy);
//                    numberProfileBatch.Add(numberProfile);
//                }
//            }
//            else
//            {
//                NumberProfile numberProfile = new NumberProfile()
//                {
//                    AccountNumber = processingNumberProfileItem.Number,
//                    FromDate = inputArgument.FromDate,
//                    ToDate = inputArgument.ToDate,
//                    IMEIs = processingNumberProfileItem.IMEIs
//                };
//                SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, inputArgument.Parameters);
//                numberProfileBatch.Add(numberProfile);
//            }

//        }

//        internal static void SetProfileAggregateValues(NumberProfile numberProfile, ProcessingNumberProfileItem processingNumberProfileItem, List<AggregateDefinition> aggregateDefinitions, int aggregatesCount, INumberProfileParameters numberProfileParameters)
//        {
//            for (int i = 0; i < aggregatesCount; i++)
//            {
//                var aggregateDef = aggregateDefinitions[i];
//                numberProfile.AggregateValues.Add(aggregateDef.KeyName, aggregateDef.Aggregation.GetResult(processingNumberProfileItem.AggregateStates[i], numberProfileParameters));
//            }
//        }

//        private static void SendNumberProfileBatch(CreateNumberProfilesInput inputArgument, AsyncActivityHandle handle, ref int numberProfilesCount, ref List<NumberProfile> numberProfileBatch)
//        {
//            numberProfilesCount += numberProfileBatch.Count;
//            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Sent", numberProfilesCount);
//            inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
//            {
//                NumberProfiles = numberProfileBatch
//            });
//            numberProfileBatch = new List<NumberProfile>();
//        }

//        protected override void OnWorkComplete(AsyncCodeActivityContext context, CreateNumberProfilesOutput result)
//        {
//            this.NumberOfSubscribers.Set(context, result.NumberOfSubscribers);
//        }

//        #region Private Classes


//        #endregion

//    }

//    public class ProcessingNumberProfileItem : IDataGroupingItem
//    {
//        public string Number { get; set; }

//        public HashSet<string> IMEIs { get; set; }

//        public List<AggregateState> AggregateStates { get; set; }
//    }

//    public class NumberProfileGroupingHandler : DataGroupingHandler
//    {
//        public List<ExecuteStrategyExecutionItem> ExecuteStrategiesExecutionItems { get; set; }

//        public bool OverridePrevious { get; set; }

//        public NumberProfileParameters Parameters { get; set; }

//        public DateTime FromDate { get; set; }

//        public DateTime ToDate { get; set; }

//        public INumberProfileProcessor NumberProfileProcessor { get; set; }

//        public override string GetItemGroupingKey(IDataGroupingHandlerGetItemGroupingKeyContext context)
//        {
//            return (context.Item as ProcessingNumberProfileItem).Number;
//        }

//        public override void UpdateExistingItemFromNew(IDataGroupingHandlerUpdateExistingFromNewContext context)
//        {
//            ProcessingNumberProfileItem existingNumberProfileItem = context.Existing as ProcessingNumberProfileItem;
//            ProcessingNumberProfileItem newNumberProfileItem = context.New as ProcessingNumberProfileItem;
//            int aggregateIndex = 0;
//            foreach (var aggregateDefinition in GetAggregateDefinitions())
//            {
//                aggregateDefinition.Aggregation.UpdateExistingFromNew(existingNumberProfileItem.AggregateStates[aggregateIndex], newNumberProfileItem.AggregateStates[aggregateIndex]);
//                aggregateIndex++;
//            }
//        }

//        public override void FinalizeGrouping(IDataGroupingHandlerFinalizeGroupingContext context)
//        {
//            var aggregateDefinitions = GetAggregateDefinitions();
//            int aggregatesCount = aggregateDefinitions.Count;
//            this.NumberProfileProcessor.Initialize();
//            foreach (ProcessingNumberProfileItem processingNumberProfileItem in context.GroupedItems)
//            {
//                if (this.ExecuteStrategiesExecutionItems != null)
//                {
//                    int index = 0;
//                    foreach (var strategyExectuionItem in this.ExecuteStrategiesExecutionItems)
//                    {
//                        var strategyProfileParameters = _strategiesAsParameters[index];
//                        NumberProfile numberProfile = new NumberProfile()
//                        {
//                            AccountNumber = processingNumberProfileItem.Number,
//                            FromDate = this.FromDate,
//                            ToDate = this.ToDate,
//                            StrategyId = strategyExectuionItem.StrategyId,
//                            StrategyExecutionID = strategyExectuionItem.StrategyExecutionId,
//                            IMEIs = processingNumberProfileItem.IMEIs
//                        };
//                        CreateNumberProfiles.SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, strategyProfileParameters);
//                        this.NumberProfileProcessor.ProcessNumberProfile(numberProfile);
//                        index++;
//                    }
//                }
//                else
//                {
//                    NumberProfile numberProfile = new NumberProfile()
//                    {
//                        AccountNumber = processingNumberProfileItem.Number,
//                        FromDate = this.FromDate,
//                        ToDate = this.ToDate,
//                        IMEIs = processingNumberProfileItem.IMEIs
//                    };
//                    CreateNumberProfiles.SetProfileAggregateValues(numberProfile, processingNumberProfileItem, aggregateDefinitions, aggregatesCount, this.Parameters);
//                    this.NumberProfileProcessor.ProcessNumberProfile(numberProfile);
//                }
//            }
//        }

//        #region Private Methods

//        List<AggregateDefinition> _aggregateDefinitions;
//        List<INumberProfileParameters> _strategiesAsParameters;
//        List<Strategy> _strategies;
//        List<AggregateDefinition> GetAggregateDefinitions()
//        {
//            if (_aggregateDefinitions == null)
//            {
//                Dictionary<int, NetType> callClassNetTypes = CreateNumberProfiles.GetCallClassNetTypes();

//                AggregateManager aggregateManager = null;
//                if (this.ExecuteStrategiesExecutionItems != null && this.ExecuteStrategiesExecutionItems.Count > 0)
//                {
//                    StrategyManager strategyManager = new StrategyManager();
//                    _strategies = this.ExecuteStrategiesExecutionItems.Select(itm => strategyManager.GetStrategy(itm.StrategyId)).ToList();
//                    _strategiesAsParameters = _strategies.Cast<INumberProfileParameters>().ToList();
//                    aggregateManager = new AggregateManager(_strategiesAsParameters);
//                }
//                else
//                    aggregateManager = new AggregateManager(new List<INumberProfileParameters> { this.Parameters });
//                _aggregateDefinitions = aggregateManager.GetAggregateDefinitions(callClassNetTypes);
//            }
//            return _aggregateDefinitions;
//        }

//        #endregion
//    }

//    public class FinalizeNumberProfileExecution : CodeActivity
//    {
//        [RequiredArgument]
//        public InArgument<List<DateTime>> CDRRangesFromDate { get; set; }

//        protected override void Execute(CodeActivityContext context)
//        {
//            System.Threading.Tasks.Parallel.ForEach(this.CDRRangesFromDate.Get(context), (fromDate) =>
//                {
//                    context.WriteTrackingMessage(LogEntryType.Information, "Started Finilizing Range '{0}'", fromDate);
//                    NumberProfileGroupingHandler groupingHandler = new NumberProfileGroupingHandler
//                    {                        
//                    };

//                    string dataGroupingName = String.Format("CDRAnalysis_CreateNumberProfile_{0}_{1}", context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, fromDate);
//                    var dataGrouper = new DistributedDataGrouper(dataGroupingName, groupingHandler);
//                    dataGrouper.StartGettingFinalResults(null);
//                    context.WriteTrackingMessage(LogEntryType.Information, "Finished Finilizing Range '{0}'", fromDate);
//                });
//        }
//    }

//    public interface INumberProfileProcessor
//    {
//        void Initialize();
//        void ProcessNumberProfile(NumberProfile numberProfile);

//        void Finalize();
//    }

//    public class NumberProfileProcessorForStrategies : INumberProfileProcessor
//    {
//        public List<int> StrategyIds { get; set; }

//        Dictionary<int, FraudManager> _fraudManagers;
//        Dictionary<int, long> _suspicionsPerStrategy;
//        List<StrategyExecutionItem> _strategyExecutionItems = new List<StrategyExecutionItem>();
//        public void Initialize()
//        {
//            _fraudManagers = new Dictionary<int, FraudManager>();
//            var strategyManager = new StrategyManager();
//            List<Strategy> strategies = this.StrategyIds.Select(itm => strategyManager.GetStrategy(itm)).ToList();

//            foreach (var strategy in strategies)
//            {
//                _fraudManagers.Add(strategy.Id, new FraudManager(strategy));
//            }
//            _suspicionsPerStrategy = this.StrategyIds.ToDictionary(itm => itm, itm => (long)0);
//        }


//        public void ProcessNumberProfile(NumberProfile numberProfile)
//        {
//            StrategyExecutionItem strategyExecutionItem;

//            if (_fraudManagers[numberProfile.StrategyId].IsNumberSuspicious(numberProfile, out strategyExecutionItem))
//            {
//                _suspicionsPerStrategy[numberProfile.StrategyId]++;
//                _strategyExecutionItems.Add(strategyExecutionItem);
//            }
//            if(_strategyExecutionItems.Count > 1000)
//            {
//                SaveStrategyExecutionItems();
//                _strategyExecutionItems.Clear();
//            }
//        }

//        private void SaveStrategyExecutionItems()
//        {
//            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
//            var dbApplyStream = dataManager.InitialiazeStreamForDBApply();
//            foreach (var itm in _strategyExecutionItems)
//            {
//                dataManager.WriteRecordToStream(itm, dbApplyStream);
//            }
//            var readyStream = dataManager.FinishDBApplyStream(dbApplyStream);
//            dataManager.ApplyStrategyExecutionItemsToDB(readyStream);
//        }


//        public void Finalize()
//        {
//            if (_strategyExecutionItems.Count > 0)
//            {
//                SaveStrategyExecutionItems();
//                _strategyExecutionItems.Clear();
//            }
//        }
//    }


//}

