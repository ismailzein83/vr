using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using TOne.Business;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes
    public class WriteReadyTableToDBInput
    {
        public ConcurrentQueue<DataTable> QueueReadyTables { get; set; }

    }

    #endregion

    public sealed class WriteReadyTableToDB : DependentAsyncActivity<WriteReadyTableToDBInput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentQueue<DataTable>> QueueReadyTables { get; set; }

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

        protected override void DoWork(WriteReadyTableToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            while (! previousActivityStatus.IsComplete || inputArgument.QueueReadyTables.Count > 0)
            {
                if (inputArgument.QueueReadyTables.Count > 0)
                {
                    ParallelIfNeeded(inputArgument.QueueReadyTables.Count, () =>
                    {
                        DataTable table;

                        while (inputArgument.QueueReadyTables.TryDequeue(out table))
                        {
                            BulkManager.Instance.Write(table.TableName, table);
                        }
                    });

                }
                Thread.Sleep(1000);
            }
        }

        protected override WriteReadyTableToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new WriteReadyTableToDBInput
            {
                QueueReadyTables = this.QueueReadyTables.Get(context)
            };
        }
    }
}
