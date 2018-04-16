using Retail.BusinessEntity.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace Retail.BusinessEntity.BP.Activities
{

    public class LoadAccountBulkActionDraftInput
    {
        public Guid BulkActionIdentifier { get; set; }
        public BaseQueue<Account> OutputQueue { get; set; }
        public BulkActionFinalState BulkActionFinalState { get; set; }

    }
    public class LoadAccountBulkActionDrafts : Vanrise.BusinessProcess.BaseAsyncActivity<LoadAccountBulkActionDraftInput>
    {
        
        #region Arguments
     
        [RequiredArgument]
        public InArgument<Guid> BulkActionIdentifier { get; set; }
        
        [RequiredArgument]
        public InArgument<BulkActionFinalState> BulkActionFinalState { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Account>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(LoadAccountBulkActionDraftInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Accounts ...");

            VRBulkActionDraftManager vrBulkActionDraftManager = new VRBulkActionDraftManager();
            var bulkActionDrafs = vrBulkActionDraftManager.GetVRBulkActionDrafts(inputArgument.BulkActionFinalState);
            throw new NotImplementedException();
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Account>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadAccountBulkActionDraftInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadAccountBulkActionDraftInput()
            {
                BulkActionIdentifier = this.BulkActionIdentifier.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                BulkActionFinalState = this.BulkActionFinalState.Get(context),
            };

        }
    }


}
