using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess
{
    public abstract class BaseAsyncActivity : AsyncCodeActivity
    {
    }

    public abstract class BaseAsyncActivity<T> : BaseAsyncActivity
    {
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Action<T> executeAction = new Action<T>(DoWork);
            context.UserState = executeAction;
            return executeAction.BeginInvoke(GetInputArgument(context), callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Action<T> executeAction = (Action<T>)context.UserState;
            executeAction.EndInvoke(result);
        }

        protected abstract T GetInputArgument(AsyncCodeActivityContext context);

        protected abstract void DoWork(T inputArgument);
    }

    public abstract class BaseAsyncActivity<T, Q> : BaseAsyncActivity
    {
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Func<T, Q> executeAction = new Func<T, Q>(DoWork);
            context.UserState = executeAction;
            return executeAction.BeginInvoke(GetInputArgument(context), callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<T, Q> executeAction = (Func<T, Q>)context.UserState;
            Q workResult = executeAction.EndInvoke(result);
            OnWorkComplete(context, workResult);
        }

        protected abstract T GetInputArgument(AsyncCodeActivityContext context);

        protected abstract Q DoWork(T inputArgument);

        protected abstract void OnWorkComplete(AsyncCodeActivityContext context, Q result);
    }
}
