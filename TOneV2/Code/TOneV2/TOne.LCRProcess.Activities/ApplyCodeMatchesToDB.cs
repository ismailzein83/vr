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

    public class ApplyCodeMatchesToDBInput
    {
        public TOneQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyCodeMatchesToDB : DependentAsyncActivity<ApplyCodeMatchesToDBInput>
    {
        [RequiredArgument]
        public InArgument<TOneQueue<Object>> InputQueue { get; set; }
               

        protected override ApplyCodeMatchesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCodeMatchesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(ApplyCodeMatchesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {
                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (preparedCodeMatches) =>
                            {
                                //Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, dtCodeMatches.Rows.Count);
                                DateTime start = DateTime.Now;
                                dataManager.ApplyCodeMatchesToDB(preparedCodeMatches);
                                totalTime += (DateTime.Now - start);
                                //Console.WriteLine("{0}: writting batch to database is done in {1}", DateTime.Now, (DateTime.Now - start));

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });
            //Console.WriteLine("{0}: ApplyCodeMatchesToDB is done in {1}", DateTime.Now, totalTime);
        }
    }
}
