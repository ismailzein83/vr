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

    public class ApplyInvalidCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyInvalidCDRsToDB : DependentAsyncActivity<ApplyInvalidCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }


        protected override ApplyInvalidCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyInvalidCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(ApplyInvalidCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedInvalidCDRs) =>
                        {
                           // Console.WriteLine("{0}: start writting {1} records to database", DateTime.Now, preparedInvalidCDRs..Count);
                           // DateTime start = DateTime.Now;
                            dataManager.ApplyInvalidCDRsToDB(preparedInvalidCDRs);
                           // totalTime += (DateTime.Now - start);
                            //Console.WriteLine("{0}: writting batch to database is done in {1}", DateTime.Now, (DateTime.Now - start));
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        
    }
}
