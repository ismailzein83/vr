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
            //var accountBEDefinitionManager = new AccountBEDefinitionManager();
            //var accountBEDefinitionSettings = accountBEDefinitionManager.GetAccountBEDefinitionSettings(inputArgument.AccountBEDefinitionId);
            //accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings", inputArgument.AccountBEDefinitionId);
            //accountBEDefinitionSettings.AccountBulkActions.ThrowIfNull("accountBEDefinitionSettings.AccountBulkActions");
          
            //if (inputArgument.AccountBulkActions != null)
            //{
            //    foreach (var accountBulkActions in inputArgument.AccountBulkActions)
            //    {

            //    }
            //}


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

    }
}
