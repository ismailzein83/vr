using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{

    #region Argument Classes
    public class UpdateNextAlertThresholdsInput
    {
        public BaseQueue<List<BalanceAccountThreshold>> InputQueue { get; set; }
    }
    #endregion

    public sealed class UpdateNextAlertThresholds : DependentAsyncActivity<UpdateNextAlertThresholdsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<List<BalanceAccountThreshold>>> InputQueue { get; set; }
        #endregion

        protected override void DoWork(UpdateNextAlertThresholdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (balanceAccountsThresholds) =>
                        {
                            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
                            dataManager.UpdateLiveBalanceThreshold(balanceAccountsThresholds);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "");
        }
        protected override UpdateNextAlertThresholdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateNextAlertThresholdsInput()
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }
    }
}
