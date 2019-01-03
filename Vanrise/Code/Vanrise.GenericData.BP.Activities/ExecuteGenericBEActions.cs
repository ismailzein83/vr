using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{
    #region Argument Classes

    public class ExecuteGenericBEActionsInput
    {
        public BaseQueue<GenericBusinessEntity> InputQueue { get; set; }
        public List<GenericBEBulkActionRuntime> GenericBEBulkActions { get; set; }
        public Guid BEDefinitionId { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }

    }
    #endregion
    public class ExecuteGenericBEActions : DependentAsyncActivity<ExecuteGenericBEActionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<GenericBusinessEntity>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<GenericBEBulkActionRuntime>> GenericBEBulkActions { get; set; }

        [RequiredArgument]
        public InArgument<Guid> BEDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<HandlingErrorOption> HandlingErrorOption { get; set; }
        #endregion

        protected override void DoWork(ExecuteGenericBEActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<GenericBEBulkActionPreparedEntity> genericBEBulkActionsPreparedEntities = new List<GenericBEBulkActionPreparedEntity>();

            if (inputArgument.GenericBEBulkActions != null && inputArgument.GenericBEBulkActions.Count>0)
            {
                var genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                var genericBEDefinitionSettings = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(inputArgument.BEDefinitionId);
                genericBEDefinitionSettings.ThrowIfNull("genericBEDefinitionSettings");
                foreach (var beBulkActions in inputArgument.GenericBEBulkActions)
                {
                    genericBEDefinitionSettings.GenericBEBulkActions.ThrowIfNull(" genericBEDefinitionSettings.GenericBEBulkActions");
                       var genericBEBulkActionDefinition = genericBEDefinitionSettings.GenericBEBulkActions.FindRecord(x => x.GenericBEBulkActionId == beBulkActions.GenericBEBulkActionId);
                    genericBEBulkActionDefinition.ThrowIfNull("genericBEBulkActionDefinition");
                    genericBEBulkActionsPreparedEntities.Add(new GenericBEBulkActionPreparedEntity
                    {
                        GenericBEBulkActionDefinition = genericBEBulkActionDefinition,
                        GenericBEBulkActionRuntime = beBulkActions
                    });
                }
            }

            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (businessEntityQueue) =>
                        {
                            counter++;
                            ExecuteAccountBulkActionsMethod(businessEntityQueue, inputArgument.BEDefinitionId, genericBEBulkActionsPreparedEntities, inputArgument.HandlingErrorOption, handle);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }
        protected override ExecuteGenericBEActionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteGenericBEActionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                GenericBEBulkActions = this.GenericBEBulkActions.Get(context),
                BEDefinitionId = this.BEDefinitionId.Get(context),
                HandlingErrorOption = this.HandlingErrorOption.Get(context),
            };
        }
     
        internal class GenericBEBulkActionPreparedEntity
        {
            public GenericBEBulkAction GenericBEBulkActionDefinition { get; set; }
            public GenericBEBulkActionRuntime GenericBEBulkActionRuntime { get; set; }
        }
        private void ExecuteAccountBulkActionsMethod(GenericBusinessEntity businessEntityQueue, Guid BEDefinitionId, List<GenericBEBulkActionPreparedEntity> genericBEBulkActionsPreparedEntities, HandlingErrorOption handlingErrorOption, AsyncActivityHandle handle)
        {
            if(genericBEBulkActionsPreparedEntities!=null && genericBEBulkActionsPreparedEntities.Count > 0) {
                foreach (var genericBEBulkActionsPreparedEntity in genericBEBulkActionsPreparedEntities)
                {
                    var genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                    DataRecordField dataRecordField = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(BEDefinitionId);
                    var actionContext = new GenericBEBulkActionRuntimeSettingsContext()
                    {
                        GenericBusinessEntity = businessEntityQueue,
                        DefinitionSettings = genericBEBulkActionsPreparedEntity.GenericBEBulkActionDefinition.Settings,
                        BEDefinitionId = BEDefinitionId
                    };
                    try
                    {
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Executing {0} for business entity '{1}'.", genericBEBulkActionsPreparedEntity.GenericBEBulkActionDefinition.Title, businessEntityQueue.FieldValues.GetRecord(dataRecordField.Name)));
                        genericBEBulkActionsPreparedEntity.GenericBEBulkActionRuntime.Settings.Execute(actionContext);
                    }
                    catch { }
                }
                //catch (Exception ex)
                //{
                //    string errorMessage = string.Format("{0} did not execute for business entity '{1}'.", genericBEBulkActionsPreparedEntity.GenericBEBulkActionDefinition.Title, businessEntityQueue.FieldValues.GetRecord(dataRecordField.Name));
                //    //var exception = Utilities.WrapException(ex, errorMessage);
                //    //switch (handlingErrorOption)
                //    //{
                //    //    case Entities.HandlingErrorOption.Skip:
                //    //        handle.SharedInstanceData.WriteBusinessHandledException(exception);
                //    //        continue;
                //    //    case Entities.HandlingErrorOption.Stop:
                //    //        throw exception;
                //    //}
                //}

                //if (actionContext.ErrorMessage != null)
                //{
                //    string errorMessage = string.Format("{0} did not execute for account '{1}'. Reason: '{2}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name, actionContext.ErrorMessage);
                //    if (!actionContext.IsErrorOccured)
                //    {
                //        handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, errorMessage);
                //    }
                //    else
                //    {
                //        switch (handlingErrorOption)
                //        {
                //            case Entities.HandlingErrorOption.Skip:
                //                handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, errorMessage);
                //                break;
                //            case Entities.HandlingErrorOption.Stop:
                //                throw new VRBusinessException(errorMessage);
                //        }
                //    }
                //}
                //else
                //{
                //    handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "{0} completed successfully for account '{1}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name);

                //}
            }

        }
    }
}
