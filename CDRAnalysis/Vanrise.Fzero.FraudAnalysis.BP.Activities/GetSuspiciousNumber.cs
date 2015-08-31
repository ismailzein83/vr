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

    public class GetSuspiciousNumberInput
    {
        public BaseQueue<NumberProfileBatch> InputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueueForFraudResult { get; set; }

        public BaseQueue<NumberProfileBatch> OutputQueue2 { get; set; }

        public List<Strategy> Strategies { get; set; }
    }

    #endregion


    public class GetSuspiciousNumber : DependentAsyncActivity<GetSuspiciousNumberInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueue { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueueForFraudResult { get; set; }

        public InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue2 { get; set; }

        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SuspiciousNumberBatch>());

            if (this.OutputQueueForFraudResult.Get(context) == null)
                this.OutputQueueForFraudResult.Set(context, new MemoryQueue<SuspiciousNumberBatch>());

            if (this.OutputQueue2.Get(context) == null)
                this.OutputQueue2.Set(context, new MemoryQueue<NumberProfileBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GetSuspiciousNumberInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Collecting Suspicious Numbers ");

            Dictionary<int, FraudManager> fraudManagers = new Dictionary<int, FraudManager>();
            ISuspiciousNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();

                foreach (var strategy in inputArgument.Strategies)
                {
                   fraudManagers.Add(strategy.Id, new FraudManager(strategy));
                }
                int numberProfilesProcessed = 0;
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (item) =>
                            {
                                List<NumberProfile> numberProfiles = new List<NumberProfile>();
                                List<SuspiciousNumber> suspiciousNumbers = new List<SuspiciousNumber>();

                                foreach (NumberProfile number in item.NumberProfiles)
                                {
                                    SuspiciousNumber sNumber = new SuspiciousNumber();

                                    if (fraudManagers[number.StrategyId].IsNumberSuspicious(number, out sNumber, number.StrategyId))
                                    {
                                        suspiciousNumbers.Add(sNumber);
                                        //numberProfiles.Add(number);
                                    }

                                    numberProfiles.Add(number);
                                }
                                if (suspiciousNumbers.Count > 0)
                                {
                                    List<AccountCaseType> cases = new List<AccountCaseType>();
                                                foreach (var i in suspiciousNumbers)
                                                {
                                                    cases.Add(  new AccountCaseType(){AccountNumber= i.Number, StrategyId= i.StrategyId, SuspicionLevelID= i.SuspicionLevel});
                                                }
                                                dataManager.UpdateSusbcriberCases(cases);

                                     inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch
                                    {
                                        SuspiciousNumbers = suspiciousNumbers
                                    });
                                     inputArgument.OutputQueueForFraudResult.Enqueue(new SuspiciousNumberBatch
                                     {
                                         SuspiciousNumbers = suspiciousNumbers
                                     });

                                }
                                   
                                if (numberProfiles.Count > 0)
                                    inputArgument.OutputQueue2.Enqueue(new NumberProfileBatch
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

        protected override GetSuspiciousNumberInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetSuspiciousNumberInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                OutputQueueForFraudResult = this.OutputQueueForFraudResult.Get(context),
                OutputQueue2 = this.OutputQueue2.Get(context),
                Strategies = this.Strategies.Get(context)
            };
        }
    }
}
