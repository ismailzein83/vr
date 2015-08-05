using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
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
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Saving Imported CDRs from Transit Database to Real Database");
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            dataManager.SaveCDRsToDB(item.cdrs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finshed Saving Imported CDRs from Transit Database to Real Database");
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
