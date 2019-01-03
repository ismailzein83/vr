using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
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
            VRBulkActionDraftManager vrBulkActionDraftManager = new VRBulkActionDraftManager();
            var bulkActionDrafts = vrBulkActionDraftManager.GetVRBulkActionDrafts(inputArgument.BulkActionFinalState);
          

            if (bulkActionDrafts != null && bulkActionDrafts.Count() != 0)
            {
                GenericBusinessEntityManager genericBEManager = new GenericBusinessEntityManager();
                GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                foreach (var bulkActionDraft in bulkActionDrafts)
                {
                    Guid genericBEId;
                    if (Guid.TryParse(bulkActionDraft.ItemId, out genericBEId))
                    {
                        var businessEntity = genericBEManager.GetGenericBusinessEntity(genericBEId, inputArgument.BEDefinitionId);
                        DataRecordField dataRecordField = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(inputArgument.BEDefinitionId);
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Loading business entity '{0}'.", businessEntity.FieldValues.GetRecord(dataRecordField.Name)));
                        inputArgument.OutputQueue.Enqueue(businessEntity);
                    }
                    else
                    {
                        handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Error, "An error had occured while parsing business entity id for bulk action draft od id {0}.", bulkActionDraft.ItemId);
                    }
                }
            }
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
