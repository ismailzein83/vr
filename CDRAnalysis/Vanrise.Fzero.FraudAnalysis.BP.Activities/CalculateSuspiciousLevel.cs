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

    public class CalculateSuspiciousLevelInput
    {
        public BaseQueue<NumberCriteriaBatch> InputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueue { get; set; }

        public Strategy strategy { get; set; }
    }

    #endregion

    public class CalculateSuspiciousLevel : DependentAsyncActivity<CalculateSuspiciousLevelInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberCriteriaBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<Strategy> strategy { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SuspiciousNumberBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(CalculateSuspiciousLevelInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            //FraudManager manager = new FraudManager(inputArgument.strategy);
            //int batchSize = 20;
            //DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            //{
            //    bool hasItem = false;
            //    do
            //    {
            //        List<SuspiciousNumber> sNumbers = new List<SuspiciousNumber>();
            //        hasItem = inputArgument.InputQueue.TryDequeue(
            //            (item) =>
            //            {
            //                SuspiciousNumber sNumber = new SuspiciousNumber();
            //                if (manager.IsNumberSuspicious(item.criteriaValues, item.number, out sNumber))
            //                {
            //                    sNumbers.Add(sNumber);
            //                }
            //                if (sNumbers.Count >= batchSize)
            //                {
            //                    inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch()
            //                    {
            //                        suspiciousNumbers = sNumbers
            //                    });
            //                    sNumbers = new List<SuspiciousNumber>();
            //                }

            //            });
            //        if (sNumbers.Count > 0)
            //        {
            //            inputArgument.OutputQueue.Enqueue(new SuspiciousNumberBatch()
            //            {
            //                suspiciousNumbers = sNumbers
            //            });
            //        }
            //    }
            //    while (!ShouldStop(handle) && hasItem);
            //});
        }

        protected override CalculateSuspiciousLevelInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CalculateSuspiciousLevelInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                strategy = this.strategy.Get(context)
            };
        }
    }
}
