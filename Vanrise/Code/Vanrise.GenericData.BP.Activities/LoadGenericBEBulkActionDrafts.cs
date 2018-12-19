using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{

    public class LoadGenericBEBulkActionDraftInput
    {
        public BaseQueue<GenericBusinessEntity> OutputQueue { get; set; }
        public BulkActionFinalState BulkActionFinalState { get; set; }
        public Guid BEDefinitionId { get; set; }
    }
    public class LoadGenericBEBulkActionDrafts : Vanrise.BusinessProcess.BaseAsyncActivity<LoadGenericBEBulkActionDraftInput>
    {
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> BEDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<BulkActionFinalState> BulkActionFinalState { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<GenericBusinessEntity>> OutputQueue { get; set; }

        #endregion


        protected override void DoWork(LoadGenericBEBulkActionDraftInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started loading genericBE ...");
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<GenericBusinessEntity>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadGenericBEBulkActionDraftInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadGenericBEBulkActionDraftInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                BulkActionFinalState = this.BulkActionFinalState.Get(context),
                BEDefinitionId = this.BEDefinitionId.Get(context),
            };

        }
    }
}
