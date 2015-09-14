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
    public class StoreMainCDRsToDBInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRMainBatch> InputQueue { get; set; }

        public int SwitchId { get; set; }
    }

    #endregion

    public sealed class StoreMainCDRsToDB : DependentAsyncActivity<StoreMainCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRMainBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchId { get; set; }

        protected override void DoWork(StoreMainCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (CDR) =>
                        {
                            if (CDR.MainCDRs != null) {
                                dataManager.SaveMainCDRsToDB(CDR.MainCDRs);
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override StoreMainCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new StoreMainCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                SwitchId = this.SwitchId.Get(context),
            };
        }
    }
}
