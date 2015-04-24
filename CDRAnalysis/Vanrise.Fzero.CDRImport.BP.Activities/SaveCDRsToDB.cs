using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    #region Arguments Classes

    public class SaveCDRsToDBInput
    {
        public BaseQueue<ImportedCDRBatch> InputQueue { get; set; }

    }

    #endregion

    public class SaveCDRsToDB : DependentAsyncActivity<SaveCDRsToDBInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<ImportedCDRBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(SaveCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start SaveCDRsToDB.DoWork.Start {0}", DateTime.Now);
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.Dequeued {0}", DateTime.Now);
                            dataManager.SaveCDRsToDB(item.cdrs);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.SavedtoDB {0}", DateTime.Now);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.End {0}", DateTime.Now);
        }

        protected override SaveCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
