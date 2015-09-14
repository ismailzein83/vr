using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{

    #region Argument Classes
    public class StoreInvalidCDRsToDBInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRInvalidBatch> InputQueue { get; set; }

        public int SwitchId { get; set; }
    }

    #endregion

    public sealed class StoreInvalidCDRsToDB : DependentAsyncActivity<StoreInvalidCDRsToDBInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRInvalidBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchId { get; set; }

        protected override void DoWork(StoreInvalidCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdr) =>
                        {
                            if (cdr.InvalidCDRs != null)
                            {
                                dataManager.SaveInvalidCDRsToDB(cdr.InvalidCDRs);
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }


        protected override StoreInvalidCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new StoreInvalidCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                SwitchId = this.SwitchId.Get(context),
            };
        }
    }
}
