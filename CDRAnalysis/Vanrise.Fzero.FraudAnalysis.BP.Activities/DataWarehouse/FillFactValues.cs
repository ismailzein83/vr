﻿using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Data;
using System;
using System.Globalization;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class FillFactValuesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<DWFactBatch> OutputQueue { get; set; }

        public DWDimensionDictionary BTSs { get; set; }

        public DWAccountCaseDictionary AccountCases { get; set; }

        public DWTimeDictionary Times { get; set; }



    }

    public class FillFactValuesOutput
    {
        public List<DWDimension> ToBeInsertedBTSs { get; set; }

        public List<DWTime> ToBeInsertedTimes { get; set; }

    }


    # endregion


    public class FillFactValues : DependentAsyncActivity<FillFactValuesInput, FillFactValuesOutput>
    {
        #region Arguments




        [RequiredArgument]
        public InArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DWFactBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> BTSs { get; set; }

        [RequiredArgument]
        public InArgument<DWAccountCaseDictionary> AccountCases { get; set; }

        [RequiredArgument]
        public InArgument<DWTimeDictionary> Times { get; set; }


        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedBTSs { get; set; }


        [RequiredArgument]
        public InOutArgument<List<DWTime>> ToBeInsertedTimes { get; set; }




        # endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<DWFactBatch>());

            base.OnBeforeExecute(context, handle);
        }

        private static void CheckIfTimeAddedorAdd(DWTimeDictionary dwTimeDictionary, List<DWTime> ToBeInsertedTimes, DateTime givenTime)
        {
            DWTime dwTime;
            DateTime dateInstance = new DateTime(givenTime.Year, givenTime.Month, givenTime.Day, givenTime.Hour, givenTime.Minute, 0);

            if (!dwTimeDictionary.TryGetValue(dateInstance, out dwTime))
            {
                dwTime = new DWTime()
                {
                    DateInstance = dateInstance,
                    Day = givenTime.Day,
                    Hour = givenTime.Hour,
                    Month = givenTime.Month,
                    Week = GetWeekOfMonth(givenTime),
                    Year = givenTime.Year,
                    MonthName = givenTime.ToString("MMMM"),
                    DayName = givenTime.ToString("dddd")
                };
                dwTimeDictionary.Add(dateInstance, dwTime);
                ToBeInsertedTimes.Add(dwTime);
            }
        }



        public static int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }


        private static List<DWFact> SendDWFacttoOutputQueue(FillFactValuesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<DWFact> dwFactBatch, bool IsLastBatch)
        {
            if (dwFactBatch.Count >= batchSize || IsLastBatch)
            {
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Data warehouse CDRs Sent", dwFactBatch.Count());
                inputArgument.OutputQueue.Enqueue(new DWFactBatch()
                {
                    DWFacts = dwFactBatch
                });
                dwFactBatch = new List<DWFact>();
            }
            return dwFactBatch;
        }

        protected override FillFactValuesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FillFactValuesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                AccountCases = this.AccountCases.Get(context),
                BTSs = this.BTSs.Get(context),
                Times = this.Times.Get(context)
            };
        }

        protected override FillFactValuesOutput DoWorkWithResult(FillFactValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DWFactBatchSize"]);
            List<DWFact> dwFactBatch = new List<DWFact>();

            CallClassManager callClassManager = new CallClassManager();
            IEnumerable<CallClass> callClasses = callClassManager.GetClasses();

            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> strategies = strategyManager.GetAll();

            int LastBTSId = 0;
            if (inputArgument.BTSs.Count() > 0)
                LastBTSId = inputArgument.BTSs.Last().Key;


            List<DWDimension> ToBeInsertedBTSs = new List<DWDimension>();
            List<DWTime> ToBeInsertedTimes = new List<DWTime>();

            var accountCases = inputArgument.AccountCases.Values;

            int cdrsCount = 0;
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
                                DWFact dwFact = new DWFact();
                                dwFact.CallType = cdr.CallType;
                                dwFact.DurationInSeconds = cdr.DurationInSeconds;
                                dwFact.IMEI = cdr.IMEI;
                                dwFact.MSISDN = cdr.MSISDN;
                                dwFact.SubscriberType = cdr.SubscriberType;

                                if (cdr.CallClassId.HasValue)
                                {
                                    CallClass callClass = CallClassManager.GetCallClassById(cdr.CallClassId.Value);
                                    if (callClass != null)
                                    {
                                        dwFact.CallClassId = callClass.Id;
                                        dwFact.NetworkType = callClass.NetType;
                                    }
                                }


                                if (!string.IsNullOrEmpty(cdr.BTS))
                                {
                                    DWDimension bts;

                                    if (inputArgument.BTSs.Where(x => x.Value.Description == cdr.BTS).Count() > 0)
                                    {
                                        bts = inputArgument.BTSs.Where(x => x.Value.Description == cdr.BTS).First().Value;
                                    }
                                    else
                                    {
                                        bts = new DWDimension() { Id = ++LastBTSId, Description = cdr.BTS };
                                        inputArgument.BTSs.Add(bts.Id, bts);
                                        ToBeInsertedBTSs.Add(bts);
                                    }
                                    dwFact.BTSId = bts.Id;
                                }


                                dwFact.ConnectDateTime = cdr.ConnectDateTime;

                                CheckIfTimeAddedorAdd(inputArgument.Times, ToBeInsertedTimes, cdr.ConnectDateTime);


                                if (accountCases.Count() > 0)
                                {
                                    var accountCase = accountCases.FirstOrDefault(x => x.AccountNumber == cdr.MSISDN && cdr.ConnectDateTime >=x.FromDate && cdr.ConnectDateTime<=x.ToDate);
                                    if (accountCase != null)
                                    {
                                        dwFact.SuspicionLevel = accountCase.SuspicionLevel;
                                        dwFact.Period = accountCase.PeriodID;

                                        Strategy strategy = new Strategy();
                                        strategy = strategies.Where(x => x.Id == accountCase.StrategyID).First();

                                        dwFact.StrategyId = accountCase.StrategyID;
                                        dwFact.StrategyKind = (strategy.IsDefault ? StrategyKindEnum.SystemBuiltIn : StrategyKindEnum.UserDefined);
                                        dwFact.StrategyUserId = strategy.UserId;
                                        dwFact.CaseId = accountCase.CaseID;
                                        dwFact.CaseStatus = accountCase.CaseStatus;
                                        dwFact.CaseUserId = accountCase.CaseUser;
                                        dwFact.CaseGenerationTime = accountCase.CaseGenerationTime;

                                        CheckIfTimeAddedorAdd(inputArgument.Times, ToBeInsertedTimes, accountCase.CaseGenerationTime);

                                    }
                                }
                                dwFactBatch.Add(dwFact);
                            }
                            cdrsCount += cdrs.Count;

                            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, false);

                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "facts filled for {0} cdrs", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);

            });




            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, true);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

            return new FillFactValuesOutput
            {

                ToBeInsertedBTSs = ToBeInsertedBTSs,
                ToBeInsertedTimes = ToBeInsertedTimes
            };

        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FillFactValuesOutput result)
        {
            this.ToBeInsertedBTSs.Set(context, result.ToBeInsertedBTSs);

            this.ToBeInsertedTimes.Set(context, result.ToBeInsertedTimes);
        }
    }
}
