﻿using System;
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

        [RequiredArgument]
        public InArgument<bool> IncludeWhiteList { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());



            base.OnBeforeExecute(context, handle);
        }

        private bool GetWhiteNumbers(string fromAccountNumber, out HashSet<string> whiteListNumbers, out string maxWhiteListNumber, bool IncludeWhiteList)
        {
            if (IncludeWhiteList)
            {
                maxWhiteListNumber = null;
                whiteListNumbers = null;
                return false;
            }

            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
             List<CaseStatus> caseStatuses = new List<CaseStatus>();
            caseStatuses.Add(CaseStatus.ClosedWhiteList);

            int nbOfRecords = 2;

            List<string> numbers = dataManager.GetAccountNumberByStatus(caseStatuses, fromAccountNumber, nbOfRecords);

            if (numbers != null && numbers.Count >= 0)
            {
                maxWhiteListNumber = numbers.Max();
                whiteListNumbers = numbers.ToHashSet();
                return whiteListNumbers.Count >= nbOfRecords;
            }
            else
            {
                maxWhiteListNumber = null;
                whiteListNumbers = null;
                return false;
            }
        }

        protected override void DoWork(CreateNumberProfilesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            HashSet<string> whiteListNumbers = null;
            string maxWhiteListNumber = null;
            bool hasMoreWhiteListNumbers = true;

            IClassDataManager manager = FraudDataManagerFactory.GetDataManager<IClassDataManager>();
            IStrategyDataManager strategyManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"]);
            var callClasses = manager.GetCallClasses();

            List<Strategy> strategies = new List<Strategy>();
            if (inputArgument.StrategiesExecutionInfo != null)
                foreach (var i in inputArgument.StrategiesExecutionInfo)
                {
                    strategies.Add(i.Strategy);
                }

            var aggregateDefinitions = strategies.Count > 0 ?
                new AggregateManager(strategies as IEnumerable<INumberProfileParameters>).GetAggregateDefinitions(callClasses)
                :
                new AggregateManager(new List<INumberProfileParameters> { inputArgument.Parameters }).GetAggregateDefinitions(callClasses)
                ;
            string currentAccountNumber = null;
            bool isCurrentAccountNumberInWhiteList = false;

            HashSet<string> IMEIs = new HashSet<string>();


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
                                    if (currentAccountNumber != null && (!isCurrentAccountNumberInWhiteList )   )
                                    {
                                        FinishNumberProfileProcessing(currentAccountNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, batchSize, aggregateDefinitions, IMEIs);
                                    }
                                    IMEIs = new HashSet<string>();

                                    currentAccountNumber = cdr.MSISDN;

                                    if ((String.Compare(maxWhiteListNumber, currentAccountNumber) < 0 && hasMoreWhiteListNumbers) )
                                    {
                                        hasMoreWhiteListNumbers = GetWhiteNumbers(currentAccountNumber, out whiteListNumbers, out maxWhiteListNumber, inputArgument.IncludeWhiteList);
                                    }

                                    if ((whiteListNumbers != null && whiteListNumbers.Contains(currentAccountNumber) ))
                                        isCurrentAccountNumberInWhiteList = true;
                                    else
                                        isCurrentAccountNumberInWhiteList = false;


                                    if (!isCurrentAccountNumberInWhiteList )
                                    {
                                        foreach (var aggregateDef in aggregateDefinitions)
                                        {
                                            aggregateDef.Aggregation.Reset();
                                            aggregateDef.Aggregation.EvaluateCDR(cdr);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!isCurrentAccountNumberInWhiteList )
                                    {
                                        IMEIs.Add(cdr.IMEI);
                                        foreach (var aggregateDef in aggregateDefinitions)
                                        {
                                            aggregateDef.Aggregation.EvaluateCDR(cdr);
                                        }
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
                FinishNumberProfileProcessing(currentAccountNumber, ref numberProfileBatch, ref numberProfilesCount, inputArgument, handle, 0, aggregateDefinitions, IMEIs);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");


        }

        private void FinishNumberProfileProcessing(string accountNumber, ref List<NumberProfile> numberProfileBatch, ref int numberProfilesCount, CreateNumberProfilesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<AggregateDefinition> AggregateDefinitions, HashSet<string> IMEIs)
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
                        StrategyExecutionID = strategyExecutionInfo.StrategyExecution.ID,
                        IMEIs = IMEIs
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
                    ToDate = inputArgument.ToDate,
                    IMEIs = IMEIs
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
                Parameters = this.Parameters.Get(context),
                IncludeWhiteList = this.IncludeWhiteList.Get(context)
            };
        }

    }
}
