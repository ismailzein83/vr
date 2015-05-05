using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Strategy strategy { get; set; }
    }

    #endregion


    public class GetSuspiciousNumber : DependentAsyncActivity<GetSuspiciousNumberInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<Strategy> strategy { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SuspiciousNumberBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GetSuspiciousNumberInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Console.WriteLine("GetSuspiciousNumber: (1) " + DateTime.Now.ToString());

            FraudManager manager = new FraudManager(inputArgument.strategy);

            Console.WriteLine("GetSuspiciousNumber: (2) " + DateTime.Now.ToString());

            int index = 0;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    index++;

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            Console.WriteLine("GetSuspiciousNumber: (3) " + DateTime.Now.ToString() + ", index : " + index.ToString());

                            List<SuspiciousNumber> sNumbers = new List<SuspiciousNumber>();

                            foreach (NumberProfile number in item.numberProfiles)
                            {
                                Console.WriteLine("GetSuspiciousNumber: (4) " + DateTime.Now.ToString() + ", index : " + index.ToString());
                                SuspiciousNumber sNumber = new SuspiciousNumber();
                                if (manager.IsNumberSuspicious(number, out sNumber))
                                {
                                    Console.WriteLine("GetSuspiciousNumber: (5) " + DateTime.Now.ToString() + ", index : " + index.ToString());
                                    sNumbers.Add(sNumber);   
                                }
                            }
                            Console.WriteLine("GetSuspiciousNumber: (6) " + DateTime.Now.ToString() + ", index : " + index.ToString());
                            if (sNumbers.Count > 0)
                            {
                                Console.WriteLine("GetSuspiciousNumber: (7) " + DateTime.Now.ToString() + ", index : " + index.ToString());
                                inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch() { 
                                    suspiciousNumbers = sNumbers
                                });
                                Console.WriteLine("GetSuspiciousNumber: (8) " + DateTime.Now.ToString() + ", index : " + index.ToString());
                            }

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            Console.WriteLine("GetSuspiciousNumber: (9) " + DateTime.Now.ToString());
        }

        protected override GetSuspiciousNumberInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GetSuspiciousNumberInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                strategy = this.strategy.Get(context)
            };
        }
    }
}
