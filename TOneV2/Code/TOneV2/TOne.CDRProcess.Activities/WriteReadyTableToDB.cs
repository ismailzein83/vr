using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using TOne.Business;

namespace TOne.CDRProcess.Activities
{

    public sealed class WriteReadyTableToDB : AsyncCodeActivity
    {
        [RequiredArgument]
        public InArgument<ConcurrentQueue<DataTable>> QueueReadyTables { get; set; }

        [RequiredArgument]
        public InArgument<CDRProcessingTasksStatus> TasksStatus { get; set; }
               
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Action<ConcurrentQueue<DataTable>, CDRProcessingTasksStatus> executeAction = new Action<ConcurrentQueue<DataTable>, CDRProcessingTasksStatus>(DoWork);
            context.UserState = executeAction;

            return executeAction.BeginInvoke(this.QueueReadyTables.Get(context), this.TasksStatus.Get(context), callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Action<ConcurrentQueue<DataTable>, CDRProcessingTasksStatus> executeAction = (Action<ConcurrentQueue<DataTable>, CDRProcessingTasksStatus>)context.UserState;
            executeAction.EndInvoke(result);
        }

        void DoWork(ConcurrentQueue<DataTable> queueTables, CDRProcessingTasksStatus tasksStatus)
        {
            while (!tasksStatus.CreatingDataTableComplete || queueTables.Count > 0)
            {
                if (queueTables.Count > 0)
                {
                    ParallelIfNeeded(queueTables.Count, () =>
                    {
                        DataTable table;

                        while (queueTables.TryDequeue(out table))
                        {
                            BulkManager.Instance.Write(table.TableName, table);
                        }
                    });
                   
                }
                Thread.Sleep(1000);
            }
        }

        void ParallelIfNeeded(int nbOfAction, Action action)
        {
            if (nbOfAction == 1)
                action();
            else
                System.Threading.Tasks.Parallel.For(0, Math.Min(nbOfAction, 5), (i) =>
                {
                    action();
                });
        }
    }
}
