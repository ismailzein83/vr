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
    public class StoreCDRsToDBInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBatch> InputQueue { get; set; }

        public int SwitchId { get; set; }
    }

    #endregion

    public sealed class StoreCDRsToDB : DependentAsyncActivity<StoreCDRsToDBInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchId { get; set; }

        protected override void DoWork(StoreCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (CDR) =>
                        {
                            Object preparedMainCDRs = dataManager.PrepareCDRsForDBApply(CDR.CDRs, CDR.SwitchId);
                            dataManager.ApplyCDRsToDB(preparedMainCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override StoreCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new StoreCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                SwitchId = this.SwitchId.Get(context),
            };
        }
    }
}
