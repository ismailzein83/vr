using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using System.Collections.Concurrent;
using TOne.Entities;
using Vanrise.BusinessProcess;
using System.Threading;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes
    public class PrepareCodeMatchesForDBApplyInput
    {
        public ConcurrentQueue<List<CodeMatch>> InputQueue { get; set; }

        public bool IsFuture { get; set; }

        public ConcurrentQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareCodeMatchesForDBApply : DependentAsyncActivity<PrepareCodeMatchesForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentQueue<List<CodeMatch>>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InOutArgument<ConcurrentQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new ConcurrentQueue<object>());
            base.OnBeforeExecute(context, handle);
        }
        
        protected override PrepareCodeMatchesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeMatchesForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                IsFuture = this.IsFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(DependentAsyncActivityInputArg<PrepareCodeMatchesForDBApplyInput> inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(inputArgument, handle, () =>
            {
                List<CodeMatch> codeMatches;
                while (!ShouldStop(handle) && inputArgument.Input.InputQueue.TryDequeue(out codeMatches))
                {
                    //Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
                    DateTime start = DateTime.Now;
                    Object preparedCodeMatches = dataManager.PrepareCodeMatchesForDBApply(codeMatches, inputArgument.Input.IsFuture);
                    inputArgument.Input.OutputQueue.Enqueue(preparedCodeMatches);
                    totalTime += (DateTime.Now - start);
                    Console.WriteLine("{0}: Preparing {1} records for DB Apply is done in {2}", DateTime.Now, codeMatches.Count, (DateTime.Now - start));
                }
            });
            Console.WriteLine("{0}: PrepareCodeMatchesForDBApply is done in {1}", DateTime.Now, totalTime);
        }
    }
}
