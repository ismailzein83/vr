using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;

namespace Vanrise.BusinessProcess.WFActivities
{
    public class ExecuteChildActivityInput<T>
    {
        public List<Object> WFExtensions { get; set; }

        public string ChildFQTN { get; set; }

        public T ChildInputArgument { get; set; }        
    }

    public class ExecuteChildActivityOutput<Q>
    {
        public Q ChildOutputArgument { get; set; }
    }

    public sealed class ExecuteChildActivity<T,Q> : BaseAsyncActivity<ExecuteChildActivityInput<T>, ExecuteChildActivityOutput<Q>>
    {
        public InArgument<string> ChildFQTN { get; set; }

        public InArgument<T> ChildInputArgument { get; set; }

        public OutArgument<Q> ChildOutputArgument { get; set; }

        protected override ExecuteChildActivityInput<T> GetInputArgument(AsyncCodeActivityContext context)
        {
            ExecuteChildActivityInput<T> input = new ExecuteChildActivityInput<T>
            {
                ChildFQTN = this.ChildFQTN.Get(context),
                ChildInputArgument = this.ChildInputArgument.Get(context)
            };
            input.WFExtensions = BuildListFromNonNull(context.GetExtension<BPSharedInstanceData>(), context.GetExtension<ConsoleTracking>(), context.GetExtension<ActivityEventsTracking>());
            return input;
        }

        List<Object> BuildListFromNonNull(params Object[] items)
        {
            return items.Where(itm => itm != null).ToList();
        }

        protected override ExecuteChildActivityOutput<Q> DoWorkWithResult(ExecuteChildActivityInput<T> inputArgument, AsyncActivityHandle handle)
        {
            string childActivityFQTN = inputArgument.ChildFQTN;
            Type childActivityType = Type.GetType(childActivityFQTN);
            if (childActivityType == null)
                throw new NullReferenceException(String.Format("childActivityType '{0}'", childActivityFQTN));
            Activity childActivity = Activator.CreateInstance(childActivityType) as Activity;
            if (childActivity == null)
                throw new Exception(String.Format("childActivity should be of type Activity. it is of type '{0}'", childActivityFQTN));
            WorkflowApplication wfApp = new WorkflowApplication(childActivity);
            foreach (var extension in inputArgument.WFExtensions)
            {
                wfApp.Extensions.Add(extension);
            }
            var inputs = new Dictionary<string, object>();
            inputs.Add("Input", inputArgument.ChildInputArgument);
            wfApp.Run();

            bool isChildCompleted = false;
            ExecuteChildActivityOutput<Q> output = new ExecuteChildActivityOutput<Q>();
            Exception exception = null;
            bool hasError = false;
            wfApp.Completed = (e) =>
            {
                object childOutputArgument = null;
                if (e.Outputs != null)
                    e.Outputs.TryGetValue("Output", out childOutputArgument);
                output.ChildOutputArgument = childOutputArgument != null ? (Q)childOutputArgument : default(Q);
                if (e.CompletionState != ActivityInstanceState.Closed)
                {
                    exception = e.TerminationException;
                    hasError = true;
                }
                isChildCompleted = true;
            };
            while (!isChildCompleted)
            {
                Thread.Sleep(500);
            }
            if (hasError)
            {
                if (exception != null)
                    throw exception;
                else
                    throw new Exception(String.Format("Child Activity '{0}' failed to complete", childActivityFQTN));
            }
            return output;
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteChildActivityOutput<Q> result)
        {
            this.ChildOutputArgument.Set(context, result.ChildOutputArgument);
        }
    }
}
