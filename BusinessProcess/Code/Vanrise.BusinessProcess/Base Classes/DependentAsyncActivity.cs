using System;
using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vanrise.BusinessProcess
{
    public abstract class DependentAsyncActivity<T, Q> : BaseAsyncActivity<Tuple<T, AsyncActivityStatus>, Q>
    {
        //[RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousActivityStatus { get; set; }


        protected override Tuple<T, AsyncActivityStatus> GetInputArgument(AsyncCodeActivityContext context)
        {
            return new Tuple<T, AsyncActivityStatus>(GetInputArgument2(context), this.PreviousActivityStatus.Get(context));
        }

        protected void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle, Action actionToDo)
        {
            while (previousActivityStatus != null && !previousActivityStatus.IsComplete && !ShouldStop(handle))
            {
                actionToDo();
                Thread.Sleep(1000);
            }
            if (!ShouldStop(handle))
                actionToDo();
        }

        protected override Q DoWorkWithResult(Tuple<T, AsyncActivityStatus> inputArgument, AsyncActivityHandle handle)
        {
            return DoWorkWithResult(inputArgument.Item1, inputArgument.Item2, handle);
        }

        protected abstract T GetInputArgument2(AsyncCodeActivityContext context);

        protected abstract Q DoWorkWithResult(T inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle);
    }

    public abstract class DependentAsyncActivity<T> : DependentAsyncActivity<T, Object>
    {
        protected override object DoWorkWithResult(T inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWork(inputArgument, previousActivityStatus, handle);
            return null;
        }

        protected abstract void DoWork(T inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle);
        protected override void OnWorkComplete(AsyncCodeActivityContext context, object result)
        {
        }
    }
}