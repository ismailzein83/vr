using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Entities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;
using System.Threading;
using System.Collections.Concurrent;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class WriteCodeMatchesToDBInput
    {
        public ConcurrentQueue<List<CodeMatch>> QueueReadyCodeMatches { get; set; }

        public bool IsFuture { get; set; }

        public AsyncActivityStatus PreviousTaskStatus { get; set; }
    }

    #endregion

    public sealed class WriteCodeMatchesToDB : BaseAsyncActivity<WriteCodeMatchesToDBInput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentQueue<List<CodeMatch>>> QueueReadyCodeMatches { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<AsyncActivityStatus> PreviousTaskStatus { get; set; }

        protected override WriteCodeMatchesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new WriteCodeMatchesToDBInput
            {
                QueueReadyCodeMatches = this.QueueReadyCodeMatches.Get(context),
                IsFuture = this.IsFuture.Get(context),
                PreviousTaskStatus = this.PreviousTaskStatus.Get(context)
            };
        }

        protected override void DoWork(WriteCodeMatchesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            while (!inputArgument.PreviousTaskStatus.IsComplete || inputArgument.QueueReadyCodeMatches.Count > 0)
            {
                List<CodeMatch> codeMatches;
                while (inputArgument.QueueReadyCodeMatches.TryDequeue(out codeMatches))
                {
                    //Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
                    DateTime start = DateTime.Now;
                    dataManager.WriteCodeMatchesDB(codeMatches, inputArgument.IsFuture);
                    totalTime += (DateTime.Now - start);
                    Console.WriteLine("{0}: writting {1} records to database is done in {2}", DateTime.Now, codeMatches.Count, (DateTime.Now - start));

                }

                Thread.Sleep(1000);
            }
            Console.WriteLine("{0}: WriteCodeMatchDataTablesToDB is done in {1}", DateTime.Now, totalTime);
        }
    }
}
