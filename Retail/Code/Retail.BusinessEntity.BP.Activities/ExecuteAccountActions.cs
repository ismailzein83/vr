using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.BP.Activities
{

    #region Argument Classes

    public class ExecuteAccountActionsInput
    {
        public BaseQueue<Account> InputQueue { get; set; }
        public List<AccountBulkActionRuntime> AccountBulkActions { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }

    }

    #endregion

    public class ExecuteAccountActions : DependentAsyncActivity<ExecuteAccountActionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<Account>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<AccountBulkActionRuntime>> AccountBulkActions { get; set; }

        [RequiredArgument]
        public InArgument<Guid> AccountBEDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<HandlingErrorOption> HandlingErrorOption { get; set; }
        #endregion

        protected override void DoWork(ExecuteAccountActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var accountBEDefinitionManager = new AccountBEDefinitionManager();
            var accountBEDefinitionSettings = accountBEDefinitionManager.GetAccountBEDefinitionSettings(inputArgument.AccountBEDefinitionId);
            accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings");
            List<AccountBulkActionPreparedEntity> accountBulkActionsPreparedEntities = new List<AccountBulkActionPreparedEntity>();
            if (inputArgument.AccountBulkActions != null)
            {
                foreach (var accountBulkActions in inputArgument.AccountBulkActions)
                {
                    var accountBulkActionDefinition = accountBEDefinitionSettings.AccountBulkActions.FindRecord(x => x.AccountBulkActionId == accountBulkActions.AccountBulkActionId);
                    accountBulkActionDefinition.ThrowIfNull("accountBulkActionDefinition");
                    accountBulkActionsPreparedEntities.Add(new AccountBulkActionPreparedEntity
                    {
                        AccountBulkActionDefinition = accountBulkActionDefinition,
                        AccountBulkActionRuntime = accountBulkActions
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
                        (accountQueue) =>
                        {
                            counter++;
                            ExecuteAccountBulkActionsMethod(accountQueue,inputArgument.AccountBEDefinitionId, accountBulkActionsPreparedEntities, inputArgument.HandlingErrorOption, handle);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }
        protected override ExecuteAccountActionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteAccountActionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                AccountBulkActions = this.AccountBulkActions.Get(context),
                AccountBEDefinitionId = this.AccountBEDefinitionId.Get(context),
                HandlingErrorOption = this.HandlingErrorOption.Get(context),
            };
        }
        internal class AccountBulkActionPreparedEntity
        {
            public AccountBulkAction AccountBulkActionDefinition { get; set; }
            public AccountBulkActionRuntime AccountBulkActionRuntime { get; set; }
        }
        private void ExecuteAccountBulkActionsMethod(Account accountQueue, Guid accountBEDefinitionId, List<AccountBulkActionPreparedEntity> accountBulkActionsPreparedEntities, HandlingErrorOption handlingErrorOption, AsyncActivityHandle handle)
        {
            foreach (var accountBulkActionPreparedEntity in accountBulkActionsPreparedEntities)
            {
                var actionContext = new AccountBulkActionSettingsContext(){
                    Account = accountQueue,
                    DefinitionSettings = accountBulkActionPreparedEntity.AccountBulkActionDefinition.Settings,
                    AccountBEDefinitionId = accountBEDefinitionId
                };
                try
                {
                    handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Executing {0} for account '{1}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name));
                    accountBulkActionPreparedEntity.AccountBulkActionRuntime.Settings.Execute(actionContext);
                }
                catch(Exception ex)
                {
                    string errorMessage = string.Format("{0} did not execute for account '{1}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name);
                    var exception = Utilities.WrapException(ex, errorMessage);
                    switch (handlingErrorOption)
                    {
                        case Entities.HandlingErrorOption.Skip:
                            handle.SharedInstanceData.WriteBusinessHandledException(exception);
                            continue;
                        case Entities.HandlingErrorOption.Stop:
                            throw exception;
                    }
                }

                if (actionContext.ErrorMessage != null)
                {
                    string errorMessage = string.Format("{0} did not execute for account '{1}'. Reason: '{2}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name, actionContext.ErrorMessage);
                    if (!actionContext.IsErrorOccured)
                    {
                        handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, errorMessage);
                    }
                    else
                    {
                        switch (handlingErrorOption)
                        {
                            case Entities.HandlingErrorOption.Skip:
                                handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, errorMessage);
                                break;
                            case Entities.HandlingErrorOption.Stop:
                                throw new VRBusinessException(errorMessage);
                        }
                    }
                }
                else
                {
                    handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "{0} completed successfully for account '{1}'.", accountBulkActionPreparedEntity.AccountBulkActionDefinition.Title, accountQueue.Name);

                }
            }
           
        }
    }
}
