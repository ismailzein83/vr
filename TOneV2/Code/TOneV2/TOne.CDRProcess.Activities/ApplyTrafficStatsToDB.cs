using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class ApplyTrafficStatsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyTrafficStatsToDB : DependentAsyncActivity<ApplyTrafficStatsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }


        protected override ApplyTrafficStatsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyTrafficStatsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(ApplyTrafficStatsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ITrafficStatisticDataManager dataManager = CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedTrafficStats) =>
                        {
                            // Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, preparedInvalidCDRs..Count);
                            // DateTime start = DateTime.Now;
                            dataManager.ApplyTrafficStatsForDB(preparedTrafficStats);
                            // totalTime += (DateTime.Now - start);
                            //Console.WriteLine("{0}: writting batch to database is done in {1}", DateTime.Now, (DateTime.Now - start));
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }


    }
}
