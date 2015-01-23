using System;
using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vanrise.BusinessProcess
{
    public class DependentAsyncActivityInputArg<T>
    {
        public T Input { get; set; }

        public AsyncActivityStatus PreviousActivityStatus { get; set; }
    }

    public abstract class DependentAsyncActivity<T, Q> : BaseAsyncActivity<DependentAsyncActivityInputArg<T>, Q>
    {
        [RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousActivityStatus { get; set; }


        protected override DependentAsyncActivityInputArg<T> GetInputArgument(AsyncCodeActivityContext context)
        {
            return new DependentAsyncActivityInputArg<T>
            {
                Input = GetInputArgument2(context),
                PreviousActivityStatus = this.PreviousActivityStatus.Get(context)
            };
        }

        protected void DoWhilePreviousRunning(DependentAsyncActivityInputArg<T> inputArgument, AsyncActivityHandle handle, Action actionToDo)
        {
            AsyncActivityStatus previousActivityStatus = inputArgument.PreviousActivityStatus;
            while (!previousActivityStatus.IsComplete && !ShouldStop(handle))
            {
                actionToDo();
                Thread.Sleep(1000);
            }
            actionToDo();
        }

        protected abstract T GetInputArgument2(AsyncCodeActivityContext context);
    }

    public abstract class DependentAsyncActivity<T> : DependentAsyncActivity<T, Object>
    {

        protected override object DoWorkWithResult(DependentAsyncActivityInputArg<T> inputArgument, AsyncActivityHandle handle)
        {
            DoWork(inputArgument, handle);
            return null;
        }

        protected abstract void DoWork(DependentAsyncActivityInputArg<T> inputArgument, AsyncActivityHandle handle);
        protected override void OnWorkComplete(AsyncCodeActivityContext context, object result)
        {
        }
    }
}