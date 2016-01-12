using System.Activities;
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

        private static void CheckIfTimeAddedorAdd(DWTimeDictionary dwTimeDictionary, List<DWTime> ToBeInsertedTimes, DateTime? givenTime)
        {
            if (!dwTimeDictionary.ContainsKey(givenTime.Value))
            {
                DateTime connectDateTime = givenTime.Value;
                DWTime dwTime = new DWTime()
                {
                    DateInstance = new DateTime(connectDateTime.Year, connectDateTime.Month, connectDateTime.Day, connectDateTime.Hour, connectDateTime.Minute, connectDateTime.Second, connectDateTime.Kind),
                    Day = connectDateTime.Day,
                    Hour = connectDateTime.Hour,
                    Month = connectDateTime.Month,
                    Week = GetWeekOfMonth(connectDateTime),
                    Year = connectDateTime.Year,
                    MonthName = connectDateTime.ToString("MMMM"),
                    DayName = connectDateTime.ToString("dddd")
                };
                dwTimeDictionary.Add(dwTime.DateInstance, dwTime);
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
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Data warehouse CDRs Sent", dwFactBatch);
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

            int LastBTSId = inputArgument.BTSs.Count;
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

                                KeyValuePair<int, DWAccountCase> accountCase = inputArgument.AccountCases.Where(x => x.Value.AccountNumber == cdr.MSISDN).OrderByDescending(x => x.Value.CaseID).FirstOrDefault();
                                if (accountCase.Key != 0)
                                {
                                    dwFact.SuspicionLevel = accountCase.Value.SuspicionLevel;
                                    dwFact.Period = accountCase.Value.PeriodID;

                                    Strategy strategy = new Strategy();
                                    strategy = strategies.Where(x => x.Id == accountCase.Value.StrategyID).First();

                                    dwFact.StrategyId = accountCase.Value.StrategyID;
                                    dwFact.StrategyKind = (strategy.IsDefault ? StrategyKindEnum.SystemBuiltIn : StrategyKindEnum.UserDefined);
                                    dwFact.StrategyUserId = strategy.UserId;
                                    dwFact.CaseId = accountCase.Key;
                                    dwFact.CaseStatus = accountCase.Value.CaseStatus;
                                    dwFact.CaseUserId = accountCase.Value.CaseUser;
                                    dwFact.CaseGenerationTime = accountCase.Value.CaseGenerationTime;

                                    CheckIfTimeAddedorAdd(inputArgument.Times, ToBeInsertedTimes, accountCase.Value.CaseGenerationTime);

                                }

                                dwFactBatch.Add(dwFact);
                            }
                            cdrsCount += cdrs.Count;

                            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, false);

                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs filled dimensions", cdrsCount);

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
