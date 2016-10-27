using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class FillFactValuesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<DWFactBatch> OutputQueue { get; set; }

        public DWBTSDictionary BTSs { get; set; }

        public DWAccountCaseDictionary AccountCases { get; set; }

        public DWTimeDictionary Times { get; set; }



    }
    # endregion


    public class FillFactValues : DependentAsyncActivity<FillFactValuesInput>
    {
        #region Arguments




        [RequiredArgument]
        public InArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DWFactBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DWBTSDictionary> BTSs { get; set; }

        [RequiredArgument]
        public InArgument<DWAccountCaseDictionary> AccountCases { get; set; }

        [RequiredArgument]
        public InArgument<DWTimeDictionary> Times { get; set; }


        # endregion

        #region Private Members
        private static void CheckIfTimeAddedorAdd(DWTimeDictionary dwTimeDictionary, List<DWTime> ToBeInsertedTimes, DateTime givenTime)
        {
            DWTime dwTime;
            DateTime dateInstance = new DateTime(givenTime.Year, givenTime.Month, givenTime.Day, givenTime.Hour, 0, 0);

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

        private static void AppliedBTSstoDB(AsyncActivityHandle handle, List<DWDimension> ToBeInsertedBTSs, IDWDimensionDataManager dimensionDataManager)
        {
            if (ToBeInsertedBTSs.Count > 0)
            {
                dimensionDataManager.TableName = "[dbo].[Dim_BTS]";
                dimensionDataManager.SaveDWDimensionsToDB(ToBeInsertedBTSs);
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "added {0} BTS(s) ", ToBeInsertedBTSs.Count);
                ToBeInsertedBTSs.Clear();
            }
        }

        private static void AppliedTimestoDB(AsyncActivityHandle handle, List<DWTime> ToBeInsertedTimes, IDWTimeDataManager timeDataManager)
        {
            if (ToBeInsertedTimes.Count > 0)
            {
                timeDataManager.SaveDWTimesToDB(ToBeInsertedTimes);
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} time(s) ", ToBeInsertedTimes.Count);
                ToBeInsertedTimes.Clear();
            }
        }

        private static int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<DWFactBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(FillFactValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWTimeDataManager timeDataManager = FraudDataManagerFactory.GetDataManager<IDWTimeDataManager>();
            IDWDimensionDataManager dimensionDataManager = FraudDataManagerFactory.GetDataManager<IDWDimensionDataManager>();

            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DWFactBatchSize"]);
            List<DWFact> dwFactBatch = new List<DWFact>();

            CallClassManager callClassManager = new CallClassManager();
            IEnumerable<CallClass> callClasses = callClassManager.GetClasses();

            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> strategies = strategyManager.GetStrategies();

            int LastBTSId = 0;
            if (inputArgument.BTSs.Count() > 0)
                LastBTSId = inputArgument.BTSs.Last().Value.Id;


            List<DWDimension> ToBeInsertedBTSs = new List<DWDimension>();
            List<DWTime> ToBeInsertedTimes = new List<DWTime>();


            int cdrsCount = 0;
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
                                    if (!inputArgument.BTSs.TryGetValue(cdr.BTS, out bts))
                                    {
                                        bts = new DWDimension() { Id = ++LastBTSId, Description = cdr.BTS };
                                        inputArgument.BTSs.Add(bts.Description, bts);
                                        ToBeInsertedBTSs.Add(bts);
                                    }
                                   
                                    dwFact.BTSId = bts.Id;
                                }


                                dwFact.ConnectDateTime = cdr.ConnectDateTime;

                                CheckIfTimeAddedorAdd(inputArgument.Times, ToBeInsertedTimes, cdr.ConnectDateTime);


                                if (inputArgument.AccountCases.Values.Count() > 0)
                                {
                                    List<DWAccountCase> values;
                                    if (inputArgument.AccountCases.TryGetValue(cdr.MSISDN, out values))
                                    {
                                        var accountCase = values.FirstOrDefault(x => cdr.ConnectDateTime >= x.FromDate && cdr.ConnectDateTime <= x.ToDate);
                                        if (accountCase != null)
                                        {
                                            dwFact.SuspicionLevel = accountCase.SuspicionLevel;
                                            dwFact.Period = accountCase.PeriodID;

                                            Strategy strategy = new Strategy();
                                            strategy = strategies.Where(x => x.Id == accountCase.StrategyID).First();

                                            dwFact.StrategyId = accountCase.StrategyID;
                                            dwFact.StrategyKind = (strategy.Settings.IsDefault ? StrategyKind.SystemBuiltIn : StrategyKind.UserDefined);
                                            dwFact.StrategyUserId = strategy.UserId;
                                            dwFact.CaseId = accountCase.CaseID;
                                            dwFact.CaseStatus = accountCase.CaseStatus;
                                            dwFact.CaseUserId = accountCase.CaseUser;
                                            dwFact.CaseGenerationTime = accountCase.CaseGenerationTime;
                                            CheckIfTimeAddedorAdd(inputArgument.Times, ToBeInsertedTimes, accountCase.CaseGenerationTime);
                                        }
                                    }
                                }
                                dwFactBatch.Add(dwFact);
                            }
                            cdrsCount += cdrs.Count;

                            AppliedTimestoDB(handle, ToBeInsertedTimes, timeDataManager);

                            AppliedBTSstoDB(handle, ToBeInsertedBTSs, dimensionDataManager);

                            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, false);

                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "facts filled for {0} cdrs", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);

            });


            AppliedTimestoDB(handle, ToBeInsertedTimes, timeDataManager);

            AppliedBTSstoDB(handle, ToBeInsertedBTSs, dimensionDataManager);

            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, true);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");
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
    }
}
