using System;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess
{
    internal class BPInstanceStore : InstanceStore
    {
        #region ctor/fields

        static IBPInstancePersistenceDataManager s_instancePersistenceDataManager = BPDataManagerFactory.GetDataManager<IBPInstancePersistenceDataManager>();
        BPInstance _bpInstance;

        internal BPInstanceStore(BPInstance bpInstance)
        {
            _bpInstance = bpInstance;
        }

        #endregion

        #region Overridden

        protected override IAsyncResult BeginTryCommand(InstancePersistenceContext context, InstancePersistenceCommand command, TimeSpan timeout, AsyncCallback callback, object state)
        {

            //The CreateWorkflowOwner command instructs the instance store to create a new instance owner bound to the instanace handle
            if (command is CreateWorkflowOwnerCommand)
            {
                context.BindInstanceOwner(Guid.NewGuid(), Guid.NewGuid());
            }
            //The SaveWorkflow command instructs the instance store to modify the instance bound to the instance handle or an instance key
            else
            {
                var saveWorkflowCommand = command as SaveWorkflowCommand;
                if (saveWorkflowCommand != null)
                {
                    if (!saveWorkflowCommand.CompleteInstance)
                    {
                        PersistWorkflow(saveWorkflowCommand);
                    }
                    else
                    {
                        DeleteWorkflow();
                    }
                }
                else
                {
                    var loadWorkflowCommand = command as LoadWorkflowCommand;
                    if (loadWorkflowCommand != null)
                    {
                        LoadWorkflow(context);
                    }
                }

            }

            return new CompletedAsyncResult<bool>(true, callback, state);
        }

        protected override bool EndTryCommand(IAsyncResult result)
        {
            return CompletedAsyncResult<bool>.End(result);
        }

        protected override bool TryCommand(InstancePersistenceContext context, InstancePersistenceCommand command, TimeSpan timeout)
        {
            return EndTryCommand(BeginTryCommand(context, command, timeout, null, null));
        }

        #endregion

        #region Private Methods

        private void PersistWorkflow(SaveWorkflowCommand saveWorkflowCommand)
        {
            IDictionary<System.Xml.Linq.XName, InstanceValue> data = saveWorkflowCommand.InstanceData;
            var serializableState = new SerializableState
            {
                Items = new List<SerializableStateItem>()
            };
            foreach (var dataItem in data)
            {
                NetDataContractSerializer s = new NetDataContractSerializer();
                using (MemoryStream str = new MemoryStream())
                {
                    s.Serialize(str, dataItem.Value.Value);
                    str.Position = 0;
                    using (StreamReader strReader = new StreamReader(str))
                    {
                        serializableState.Items.Add(new SerializableStateItem
                        {
                            KeyLocalName = dataItem.Key.LocalName,
                            KeyNamespaceName = dataItem.Key.NamespaceName,
                            SerializedData = strReader.ReadToEnd()
                        });
                    }
                }
            }
            s_instancePersistenceDataManager.InsertOrUpdateInstance(_bpInstance.ProcessInstanceID, Serializer.Serialize(serializableState));
            _bpInstance.Status = BPInstanceStatus.Waiting;
            _bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Free;
            BPDefinitionInitiator.UpdateProcessStatus(_bpInstance, true);
        }

        private void LoadWorkflow(InstancePersistenceContext context)
        {
            IDictionary<System.Xml.Linq.XName, InstanceValue> data = new Dictionary<System.Xml.Linq.XName, InstanceValue>();
            NetDataContractSerializer s = new NetDataContractSerializer();
            var serializableState = Serializer.Deserialize<SerializableState>(s_instancePersistenceDataManager.GetInstanceState(_bpInstance.ProcessInstanceID));
            foreach (var item in serializableState.Items)
            {
                Object deserializedObject = null;
                using (MemoryStream str = new MemoryStream())
                {
                    using (var writer = new StreamWriter(str))
                    {
                        writer.Write(item.SerializedData);
                        writer.Flush();
                        str.Position = 0;
                        deserializedObject = s.Deserialize(str);
                    }
                }
                data.Add(System.Xml.Linq.XName.Get(item.KeyLocalName, item.KeyNamespaceName), new InstanceValue(deserializedObject));
            }
            //load the data into the persistence Context
            context.LoadedInstance(InstanceState.Initialized, data, null, null, null);
        }

        private void DeleteWorkflow()
        {
            s_instancePersistenceDataManager.DeleteState(_bpInstance.ProcessInstanceID);
        }

        #endregion

        #region Private Classes

        private class SerializableState
        {
            public List<SerializableStateItem> Items { get; set; }
        }

        public class SerializableStateItem
        {
            public string KeyLocalName { get; set; }

            public string KeyNamespaceName { get; set; }

            public string SerializedData { get; set; }
        }

        abstract class AsyncResult : IAsyncResult
        {
            static AsyncCallback asyncCompletionWrapperCallback;
            AsyncCallback callback;
            bool completedSynchronously;
            bool endCalled;
            Exception exception;
            bool isCompleted;
            ManualResetEvent manualResetEvent;
            AsyncCompletion nextAsyncCompletion;
            object state;
            object thisLock;

            protected AsyncResult(AsyncCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
                this.thisLock = new object();
            }

            public object AsyncState
            {
                get
                {
                    return state;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (manualResetEvent != null)
                    {
                        return manualResetEvent;
                    }

                    lock (ThisLock)
                    {
                        if (manualResetEvent == null)
                        {
                            manualResetEvent = new ManualResetEvent(isCompleted);
                        }
                    }

                    return manualResetEvent;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return completedSynchronously;
                }
            }

            public bool HasCallback
            {
                get
                {
                    return this.callback != null;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return isCompleted;
                }
            }

            object ThisLock
            {
                get
                {
                    return this.thisLock;
                }
            }

            protected void Complete(bool completedSynchronously)
            {
                if (isCompleted)
                {
                    // It's a bug to call Complete twice.
                    throw new InvalidProgramException();
                }

                this.completedSynchronously = completedSynchronously;

                if (completedSynchronously)
                {
                    // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                    // we don't need to worry about a race
                    this.isCompleted = true;
                }
                else
                {
                    lock (ThisLock)
                    {
                        this.isCompleted = true;
                        if (this.manualResetEvent != null)
                        {
                            this.manualResetEvent.Set();
                        }
                    }
                }

                if (callback != null)
                {
                    try
                    {
                        callback(this);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidProgramException("Async callback threw an Exception", e);
                    }
                }
            }

            protected void Complete(bool completedSynchronously, Exception exception)
            {
                this.exception = exception;
                Complete(completedSynchronously);
            }

            static void AsyncCompletionWrapperCallback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                AsyncResult thisPtr = (AsyncResult)result.AsyncState;
                AsyncCompletion callback = thisPtr.GetNextCompletion();

                bool completeSelf = false;
                Exception completionException = null;
                try
                {
                    completeSelf = callback(result);
                }
                catch (Exception e)
                {
                    if (IsFatal(e))
                    {
                        throw;
                    }
                    completeSelf = true;
                    completionException = e;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            public static bool IsFatal(Exception exception)
            {
                while (exception != null)
                {
                    if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                        exception is ThreadAbortException ||
                        exception is AccessViolationException ||
                        exception is System.Runtime.InteropServices.SEHException)
                    {
                        return true;
                    }

                    // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                    // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                    // count as fatal.
                    if (exception is TypeInitializationException ||
                        exception is System.Reflection.TargetInvocationException)
                    {
                        exception = exception.InnerException;
                    }
                    else
                    {
                        break;
                    }
                }

                return false;
            }

            protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion callback)
            {
                this.nextAsyncCompletion = callback;
                if (AsyncResult.asyncCompletionWrapperCallback == null)
                {
                    AsyncResult.asyncCompletionWrapperCallback = new AsyncCallback(AsyncCompletionWrapperCallback);
                }
                return AsyncResult.asyncCompletionWrapperCallback;
            }

            AsyncCompletion GetNextCompletion()
            {
                AsyncCompletion result = this.nextAsyncCompletion;
                this.nextAsyncCompletion = null;
                return result;
            }

            protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
                where TAsyncResult : AsyncResult
            {
                if (result == null)
                {
                    throw new ArgumentNullException("result");
                }

                TAsyncResult asyncResult = result as TAsyncResult;

                if (asyncResult == null)
                {
                    throw new ArgumentException("Invalid AsyncResult", "result");
                }

                if (asyncResult.endCalled)
                {
                    throw new InvalidOperationException("AsyncResult already ended");
                }

                asyncResult.endCalled = true;

                if (!asyncResult.isCompleted)
                {
                    asyncResult.AsyncWaitHandle.WaitOne();
                }

                if (asyncResult.manualResetEvent != null)
                {
                    asyncResult.manualResetEvent.Close();
                }

                if (asyncResult.exception != null)
                {
                    throw asyncResult.exception;
                }

                return asyncResult;
            }

            // can be utilized by subclasses to write core completion code for both the sync and async paths
            // in one location, signaling chainable synchronous completion with the boolean result,
            // and leveraging PrepareAsyncCompletion for conversion to an AsyncCallback.
            // NOTE: requires that "this" is passed in as the state object to the asynchronous sub-call being used with a completion routine.
            protected delegate bool AsyncCompletion(IAsyncResult result);
        }

        class CompletedAsyncResult<T> : AsyncResult
        {
            T data;

            public CompletedAsyncResult(T data, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.data = data;
                Complete(true);
            }

            public static T End(IAsyncResult result)
            {
                CompletedAsyncResult<T> completedResult = AsyncResult.End<CompletedAsyncResult<T>>(result);
                return completedResult.data;
            }
        }

        #endregion
    }
}
