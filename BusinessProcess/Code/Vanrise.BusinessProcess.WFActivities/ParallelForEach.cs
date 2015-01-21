using System;
using System.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace Vanrise.BusinessProcess.WFActivities
{
    [Designer(typeof(Designer.ParallelForEachDesigner))]
    [ContentProperty("Body")]
    public sealed class ParallelForEach<T> : NativeActivity
    {
        Variable<bool> hasCompleted;
        Variable<int> _executedInstances;
        Variable<int> _runningInstances;

        CompletionCallback<bool> onConditionComplete;

        public ParallelForEach()
            : base()
        {
        }

        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IEnumerable<T>> Values
        {
            get;
            set;
        }

        [RequiredArgument]
        [DefaultValue(1)]
        public InArgument<int> InitialConcurrentInstances
        {
            get;
            set;
        }

        [RequiredArgument]
        [DefaultValue(2)]
        public InArgument<int> MaxConcurrentInstances
        {
            get;
            set;
        }

        [DefaultValue(null)]
        [DependsOn("Values")]
        public Activity<bool> CompletionCondition
        {
            get;
            set;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        [DependsOn("CompletionCondition")]
        public ActivityAction<T> Body
        {
            get;
            set;
        }

       

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            RuntimeArgument valuesArgument = new RuntimeArgument("Values", typeof(IEnumerable<T>), ArgumentDirection.In, true);
            metadata.Bind(this.Values, valuesArgument);

            RuntimeArgument initialConcurrentInstances = new RuntimeArgument("InitialConcurrentInstances", typeof(int), ArgumentDirection.In, true);
            metadata.Bind(this.InitialConcurrentInstances, initialConcurrentInstances);

            RuntimeArgument maxConcurrentInstancesArgument = new RuntimeArgument("MaxConcurrentInstances", typeof(int), ArgumentDirection.In, true);
            metadata.Bind(this.MaxConcurrentInstances, maxConcurrentInstancesArgument);

            metadata.SetArgumentsCollection(new Collection<RuntimeArgument> { valuesArgument, initialConcurrentInstances, maxConcurrentInstancesArgument });

            

            // declare the CompletionCondition as a child
            if (this.CompletionCondition != null)
            {
                metadata.SetChildrenCollection(new Collection<Activity> { this.CompletionCondition });
            }

            // declare the hasCompleted variable
            if (this.CompletionCondition != null)
            {
                if (this.hasCompleted == null)
                {
                    this.hasCompleted = new Variable<bool>();
                }

                metadata.AddImplementationVariable(this.hasCompleted);
            }

            if (this._executedInstances == null)
            {
                this._executedInstances = new Variable<int>();
                metadata.AddImplementationVariable(this._executedInstances);
            }

            if (this._runningInstances == null)
            {
                this._runningInstances = new Variable<int>();
                metadata.AddImplementationVariable(this._runningInstances);
            }

            metadata.AddDelegate(this.Body);
        }

        protected override void Execute(NativeActivityContext context)
        {
            ExecuteNext(context, this.InitialConcurrentInstances.Get(context));
        }

        void ExecuteNext(NativeActivityContext context, int maxRunningInstances)
        {
            IEnumerable<T> values = this.Values.Get(context);
            if (values == null)
            {
                throw new InvalidOperationException("ParallelForEach requires a non-null Values argument.");
            }

            IEnumerator<T> valueEnumerator = values.GetEnumerator();

            CompletionCallback onBodyComplete = new CompletionCallback(OnBodyComplete);
            int executedInstances = this._executedInstances.Get(context);
            for (int i = 0; i < executedInstances; i++)
            {
                valueEnumerator.MoveNext();
            }
            int runningInstances = this._runningInstances.Get(context);
            while (runningInstances < maxRunningInstances && valueEnumerator.MoveNext())
            {
                if (this.Body != null)
                {
                    context.ScheduleAction<T>(this.Body, valueEnumerator.Current, onBodyComplete);
                }
                executedInstances++;
                runningInstances++;
            }

            this._executedInstances.Set(context, executedInstances);
            this._runningInstances.Set(context, runningInstances);
            IDisposable disposable = valueEnumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        protected override void Cancel(NativeActivityContext context)
        {
            // If we don't have a completion condition then we can just
            // use default logic.
            if (this.CompletionCondition == null)
            {
                base.Cancel(context);
            }
            else
            {
                context.CancelChildren();
            }
        }

        void OnBodyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            this._runningInstances.Set(context, this._runningInstances.Get(context) - 1);
            ExecuteNext(context, this.MaxConcurrentInstances.Get(context));
            // for the completion condition, we handle cancelation ourselves
            if (this.CompletionCondition != null && !this.hasCompleted.Get(context))
            {
                if (completedInstance.State != ActivityInstanceState.Closed && context.IsCancellationRequested)
                {
                    // If we hadn't completed before getting canceled
                    // or one of our iteration of body cancels then we'll consider
                    // ourself canceled.
                    context.MarkCanceled();
                    this.hasCompleted.Set(context, true);
                }
                else
                {
                    if (this.onConditionComplete == null)
                    {
                        this.onConditionComplete = new CompletionCallback<bool>(OnConditionComplete);
                    }
                    context.ScheduleActivity(CompletionCondition, this.onConditionComplete);
                }
            }
        }

        void OnConditionComplete(NativeActivityContext context, ActivityInstance completedInstance, bool result)
        {
            if (result)
            {
                context.CancelChildren();
                this.hasCompleted.Set(context, true);
            }
        }
    }
}