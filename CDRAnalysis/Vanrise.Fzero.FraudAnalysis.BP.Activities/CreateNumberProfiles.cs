using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

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

        public List<string> NumberPrefixes { get; set; }


    }

    public class CreateNumberProfilesOutput
    {
        public long NumberOfSubscribers { get; set; }

    }

    #endregion

    public class CreateNumberProfiles : DependentAsyncActivity<CreateNumberProfilesInput, CreateNumberProfilesOutput>
    {

        #region Arguments

        public InArgument<List<string>> NumberPrefixes { get; set; }


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
                    for (int i = 0; i < aggregatesCount; i++)
                    {
                        var aggregateDef = aggregateDefinitions[i];
                        numberProfile.AggregateValues.Add(aggregateDef.KeyName, aggregateDef.Aggregation.GetResult(processingNumberProfileItem.AggregateStates[i], strategyExecutionInfo.Strategy));
                    }
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
                for (int i = 0; i < aggregatesCount; i++)
                {
                    var aggregateDef = aggregateDefinitions[i];
                    numberProfile.AggregateValues.Add(aggregateDef.KeyName, aggregateDef.Aggregation.GetResult(processingNumberProfileItem.AggregateStates[i], inputArgument.Parameters));
                }
                numberProfileBatch.Add(numberProfile);
            }

        }

        protected override CreateNumberProfilesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CreateNumberProfilesInput
            {
                NumberPrefixes = this.NumberPrefixes.Get(context),
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
            {
                AccountStatusManager accountStatusManager = new AccountStatusManager();
                var whiteListNumbers = accountStatusManager.GetAccountNumbersByNumberPrefixAndStatuses(new List<CaseStatusEnum> { CaseStatusEnum.ClosedWhiteList }, inputArgument.NumberPrefixes);
                whiteListNumbersHashSet = new HashSet<string>(whiteListNumbers);
            }

            IClassDataManager manager = FraudDataManagerFactory.GetDataManager<IClassDataManager>();
            IStrategyDataManager strategyManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

            var callClasses = manager.GetCallClasses();
            Dictionary<int, NetType> callClassNetTypes = new Dictionary<int, NetType>();
            if (callClasses != null)
            {
                foreach (CallClass callClass in callClasses)
                {
                    callClassNetTypes.Add(callClass.Id, callClass.NetType);
                }
            }
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
            ProcessingNumberProfileItemByNumbers processingNumberProfileItemByNumbers = new ProcessingNumberProfileItemByNumbers();

            int cdrsCount = 0;
            int cdrIndex = 0;

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
                                long msidnAsNumber;
                                if (!long.TryParse(cdr.MSISDN, out msidnAsNumber))
                                    continue;

                                ProcessingNumberProfileItem currentProcessingNumberProfileItem;
                                if (!processingNumberProfileItemByNumbers.TryGetValue(msidnAsNumber, out currentProcessingNumberProfileItem))//|| cdrProfilingCount == 2)
                                {
                                    if (whiteListNumbersHashSet.Contains(cdr.MSISDN))
                                        continue;
                                    currentProcessingNumberProfileItem = new ProcessingNumberProfileItem
                                    {
                                        Number = cdr.MSISDN,
                                        AggregateStates = aggregateManager.CreateAggregateStates(aggregateDefinitions)
                                    };
                                    processingNumberProfileItemByNumbers.Add(msidnAsNumber, currentProcessingNumberProfileItem);
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

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);

            int batchSize;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["FraudAnalysis_NumberProfileBatchSize"], out batchSize))
                batchSize = 100000;

            int numberProfilesCount = 0;
            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
            long numberOfSubscribers = 0;
            foreach (var processingNumberProfileItem in processingNumberProfileItemByNumbers.Values)
            {
                numberOfSubscribers++;
                FinishNumberProfileProcessing(processingNumberProfileItem, aggregateDefinitions, aggregatesCount, ref numberProfileBatch, inputArgument);


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

            if (numberProfileBatch.Count > 0)
            {
                numberProfilesCount += numberProfileBatch.Count;
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Number Profiles Sent", numberProfilesCount);
                inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                {
                    NumberProfiles = numberProfileBatch
                });
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");
            return new CreateNumberProfilesOutput
            {
                NumberOfSubscribers = numberOfSubscribers
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CreateNumberProfilesOutput result)
        {
            this.NumberOfSubscribers.Set(context, result.NumberOfSubscribers);
        }

        #region Private Classes
        
        private class AggregateEvaluatuationTask
        {
            public ProcessingNumberProfileItem ProcessingNumberProfileItem { get; set; }

            public CDR CDR { get; set; }
        }

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

        private class ProcessingNumberProfileItemByNumbers : BigDictionary<ProcessingNumberProfileItem>
        {
        }

        #endregion

    }

    public class BigDictionary<T>
    {
        Dictionary<long, Dictionary<long, T>> _dictionaryOfDictionaries = new Dictionary<long, Dictionary<long, T>>();
        public void Add(long key, T value)
        {
           GetDictionary(key).Add(key, value);
        }

        public IEnumerable<T> Values
        {
            get
            {
                return _dictionaryOfDictionaries.SelectMany(itm => itm.Value.Values);
            }
        }

        public bool TryGetValue(long key, out T value)
        {
            return GetDictionary(key).TryGetValue(key, out value);
        }

        Dictionary<long, T> GetDictionary(long key)
        {
            long dictionaryKey = key % 1000;
            Dictionary<long, T> dictionary;
            if(!_dictionaryOfDictionaries.TryGetValue(dictionaryKey, out dictionary))
            {
                dictionary = new Dictionary<long, T>();
                _dictionaryOfDictionaries.Add(dictionaryKey, dictionary);
            }
            return dictionary;
        }
    }
}
