using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class GetSuspiciousNumberInput
    {
        public BaseQueue<NumberProfileBatch> InputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueue { get; set; }

        public List<Strategy> Strategies { get; set; }
    }

    #endregion


    public class GetSuspiciousNumber : DependentAsyncActivity<GetSuspiciousNumberInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SuspiciousNumberBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GetSuspiciousNumberInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "GetSuspiciousNumber.DoWork.Started ");

            FraudManager manager;
            foreach(var strategy in inputArgument.Strategies)
            {
                manager = new FraudManager(strategy);
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (item) =>
                            {

                                List<SuspiciousNumber> sNumbers = new List<SuspiciousNumber>();

                                foreach (NumberProfile number in item.numberProfiles)
                                {
                                    SuspiciousNumber sNumber = new SuspiciousNumber();
                                    if (manager.IsNumberSuspicious(number, out sNumber, strategy.Id))
                                    {
                                        sNumbers.Add(sNumber);   
                                    }
                                }
                                if (sNumbers.Count > 0)
                                {
                                    inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch() { 
                                        suspiciousNumbers = sNumbers
                                    });
                                    handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "GetSuspiciousNumber.DoWork.Enqueued Count Items: {0} ", sNumbers.Count);
                                }
                                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "GetSuspiciousNumber.DoWork.Dequeued Count Items: {0} ", item.numberProfiles.Count);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });
            }
            
            



            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "GetSuspiciousNumber.DoWork.Ended ");
        }

        protected override GetSuspiciousNumberInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetSuspiciousNumberInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                Strategies = this.Strategies.Get(context)
            };
        }
    }
}
