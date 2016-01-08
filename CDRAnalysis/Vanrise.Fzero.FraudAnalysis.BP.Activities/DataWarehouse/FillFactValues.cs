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
    # endregion


    public class FillFactValues : DependentAsyncActivity<FillFactValuesInput>
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

        # endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<DWFactBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(FillFactValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DWFactBatchSize"]);
            List<DWFact> dwFactBatch = new List<DWFact>();

            CallClassManager callClassManager = new CallClassManager();
            IEnumerable<CallClass> callClasses = callClassManager.GetClasses();

            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> strategies = strategyManager.GetAll();

            int LastBTSId = inputArgument.BTSs.Count;

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


                                dwFact.CallType = (int)cdr.CallType;
                                dwFact.CDRId = cdr.Id;
                                dwFact.ConnectTime = cdr.ConnectDateTime;
                                dwFact.Duration = cdr.DurationInSeconds;
                                dwFact.IMEI = cdr.IMEI;
                                dwFact.MSISDN = cdr.MSISDN;
                                dwFact.SubscriberType = cdr.SubType;


                                CallClass callClass = callClasses.Where(x => x.Description == cdr.CallClass).FirstOrDefault();
                                if (callClass == null)
                                {
                                    dwFact.CallClass = callClass.Id;
                                    dwFact.NetworkType = (int)callClass.NetType;
                                }


                                dwFact.BTS = cdr.BTSId;
                                if (cdr.BTSId != null)
                                    if (!inputArgument.BTSs.ContainsKey(cdr.BTSId.Value))
                                    {
                                        DWDimension dwDimension = new DWDimension() { Id = ++LastBTSId, Description = cdr.BTSId.Value.ToString() };
                                        inputArgument.BTSs.Add(dwDimension.Id, dwDimension);
                                    }


                                KeyValuePair<int, DWAccountCase> accountCase = inputArgument.AccountCases.Where(x => x.Value.AccountNumber == cdr.MSISDN).OrderByDescending(x => x.Value.CaseID).FirstOrDefault();
                                if (accountCase.Key != 0)
                                {
                                    dwFact.SuspicionLevel = accountCase.Value.SuspicionLevelID;
                                    dwFact.Period = accountCase.Value.PeriodID;




                                    Strategy strategy = new Strategy();
                                    strategy = strategies.Where(x => x.Id == accountCase.Value.StrategyID).First();

                                    dwFact.Strategy = accountCase.Value.StrategyID;
                                    dwFact.StrategyKind = (strategy.IsDefault ? 0 : 1);
                                    dwFact.StrategyUser = strategy.UserId;
                                    dwFact.CaseId = accountCase.Key;
                                    dwFact.CaseStatus = accountCase.Value.CaseStatus;
                                    dwFact.CaseUser = accountCase.Value.CaseUser;
                                    dwFact.CaseGenerationTime = accountCase.Value.CaseGenerationTime;
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
    }
}
