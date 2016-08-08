using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace Vanrise.AccountBalance.BP.Activities
{
    
    #region Argument Classes
    public class LoadExpiredAlertRuleActionExecutionToDeleteInput
    {
        public BaseQueue<AlertRuleActionExecution> OutputQueue { get; set; }
    }
    #endregion
    public class LoadExpiredAlertRuleActionExecutionToDelete : BaseAsyncActivity<LoadExpiredAlertRuleActionExecutionToDeleteInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AlertRuleActionExecution>> OutputQueue { get; set; }
        #endregion

        protected override void DoWork(LoadExpiredAlertRuleActionExecutionToDeleteInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Expired Alert Rule Action Execution To Delete.");

            IAlertRuleActionExecutionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAlertRuleActionExecutionDataManager>();
            dataManager.GetAletRuleAciontExecutionsToDelete((alertRuleActionExecution) =>
            {
              inputArgument.OutputQueue.Enqueue(alertRuleActionExecution);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Expired Alert Rule Action Execution To Delete.");
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<AlertRuleActionExecution>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadExpiredAlertRuleActionExecutionToDeleteInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadExpiredAlertRuleActionExecutionToDeleteInput()
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
