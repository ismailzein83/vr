using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;
using System;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class GetSuspiciousNumberInput
    {
        public BaseQueue<NumberProfileBatch> InputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueue { get; set; }

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

        public InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue2 { get; set; }

        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SuspiciousNumberBatch>());

            if (this.OutputQueue2.Get(context) == null)
                this.OutputQueue2.Set(context, new MemoryQueue<NumberProfileBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GetSuspiciousNumberInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Started Collecting Suspicious Numbers ");

            Dictionary<int, FraudManager> fraudManagers = new Dictionary<int, FraudManager>();

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
                                        numberProfiles.Add(number);
                                    }

                                    //numbers.Add(number);
                                }
                                if (suspiciousNumbers.Count > 0)
                                    inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch
                                    {
                                        SuspiciousNumbers = suspiciousNumbers
                                    });
                                if (numberProfiles.Count > 0)
                                    inputArgument.OutputQueue2.Enqueue(new NumberProfileBatch
                                    {
                                        NumberProfiles = numberProfiles
                                    });

                                numberProfilesProcessed += item.NumberProfiles.Count;
                                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Verbose, "{0} Number Profiles Processed", numberProfilesProcessed);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });

                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Finshed Collecting Suspicious Numbers ");
        }

        protected override GetSuspiciousNumberInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetSuspiciousNumberInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                OutputQueue2 = this.OutputQueue2.Get(context),
                Strategies = this.Strategies.Get(context)
            };
        }
    }
}
