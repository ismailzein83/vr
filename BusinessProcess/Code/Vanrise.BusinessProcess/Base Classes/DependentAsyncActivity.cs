using System;
using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vanrise.Data;
using Vanrise.Queueing;

namespace Vanrise.BusinessProcess
{
    public abstract class DependentAsyncActivity<T, Q> : BaseAsyncActivity<Tuple<T, AsyncActivityStatus>, Q>
    {
        public virtual int PrepareDataForDBApplyBatchSize
        {
            get
            {
                return 100000;
            }
        }
        //[RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousActivityStatus { get; set; }


        protected override Tuple<T, AsyncActivityStatus> GetInputArgument(AsyncCodeActivityContext context)
        {
            return new Tuple<T, AsyncActivityStatus>(GetInputArgument2(context), this.PreviousActivityStatus.Get(context));
        }

        public void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle, Action actionToDo)
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

        public void PrepareDataForDBApply<R, S>(AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle, IBulkApplyDataManager<R> dataManager, BaseQueue<S> inputQueue, BaseQueue<Object> outputQueue, Func<S, IEnumerable<R>> GetItems)
        {
           //System.Threading.Tasks.Parallel.For(0, 3, (i) =>
           //{
               object dbApplyStream = null;
               int addedItemsToStream = 0;
               DoWhilePreviousRunning(previousActivityStatus, handle, () =>
               {
                   bool hasItems = false;
                   do
                   {
                       hasItems = inputQueue.TryDequeue(
                       (inputBatch) =>
                       {
                           if (dbApplyStream == null)
                               dbApplyStream = dataManager.InitialiazeStreamForDBApply();
                           foreach (var item in GetItems(inputBatch))
                           {
                               dataManager.WriteRecordToStream(item, dbApplyStream);
                               addedItemsToStream++;
                           }
                           if (addedItemsToStream >= this.PrepareDataForDBApplyBatchSize)
                           {
                               Object preparedItemsForDBApply = dataManager.FinishDBApplyStream(dbApplyStream);
                               outputQueue.Enqueue(preparedItemsForDBApply);
                               dbApplyStream = null;
                               addedItemsToStream = 0;
                           }
                       });
                   } while (!ShouldStop(handle) && hasItems);
               });
               if (addedItemsToStream > 0)
               {
                   Object preparedItemsForDBApply = dataManager.FinishDBApplyStream(dbApplyStream);
                   outputQueue.Enqueue(preparedItemsForDBApply);
               }
           //});
        }
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