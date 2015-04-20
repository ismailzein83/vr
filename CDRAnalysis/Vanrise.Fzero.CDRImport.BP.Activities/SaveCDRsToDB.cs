using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
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

    public class SaveCDRsToDB : BaseAsyncActivity<SaveCDRsToDBInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<ImportedCDRBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(SaveCDRsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            inputArgument.InputQueue.TryDequeue(
                        (CDRBatch) =>
                        {
                           // CDRBatch.cdrs

                           // Object preparedMainCDRs = dataManager.PrepareCDRsForDBApply(CDR.CDRs, CDR.SwitchId);
                           // dataManager.ApplyCDRsToDB(preparedMainCDRs);
                        });
        }

        protected override SaveCDRsToDBInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new SaveCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
