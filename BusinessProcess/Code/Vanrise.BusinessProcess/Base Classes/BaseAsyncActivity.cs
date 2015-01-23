using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess
{
    public abstract class BaseAsyncActivity<T, Q> : AsyncCodeActivity
    {
        public OutArgument<AsyncActivityStatus> Status { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            this.Status.Set(context, new AsyncActivityStatus());
            Func<T, AsyncActivityHandle, Q> executeAction = new Func<T, AsyncActivityHandle, Q>(DoWorkWithResult);
            context.UserState = executeAction;
            AsyncActivityHandle handle = new AsyncActivityHandle
                {
                    SharedInstanceData = context.GetSharedInstanceData()
                };
            OnBeforeExecute(context, handle);
            return executeAction.BeginInvoke(GetInputArgument(context), handle, callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<T, AsyncActivityHandle, Q> executeAction = (Func<T, AsyncActivityHandle, Q>)context.UserState;
            Q workResult = executeAction.EndInvoke(result);
            OnWorkComplete(context, workResult);
            this.Status.Get(context).IsComplete = true;
        }

        protected abstract T GetInputArgument(AsyncCodeActivityContext context);

        protected abstract Q DoWorkWithResult(T inputArgument, AsyncActivityHandle handle);

        protected abstract void OnWorkComplete(AsyncCodeActivityContext context, Q result);

        protected virtual void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {

        }

        protected bool ShouldStop(AsyncActivityHandle handle)
        {
            return handle.SharedInstanceData.InstanceInfo.Status != Entities.BPInstanceStatus.Running;
        }
    }

    public abstract class BaseAsyncActivity<T> : BaseAsyncActivity<T, Object>
    {
        protected override object DoWorkWithResult(T inputArgument, AsyncActivityHandle handle)
        {
            DoWork(inputArgument, handle);
            return null;
        }

        protected abstract void DoWork(T inputArgument, AsyncActivityHandle handle);

        protected override void OnWorkComplete(AsyncCodeActivityContext context, object result)
        {

        }
    }
}
