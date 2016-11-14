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
        public Guid AccountTypeId { get; set; }
        public BaseQueue<List<BalanceAccountThreshold>> InputQueue { get; set; }
    }
    #endregion

    public sealed class UpdateNextAlertThresholds : DependentAsyncActivity<UpdateNextAlertThresholdsInput>
    {

        #region Arguments

        public InArgument<Guid> AccountTypeId { get; set; }

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
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Update next alert thresholds.");
                            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
                            dataManager.UpdateLiveBalanceThreshold(inputArgument.AccountTypeId, balanceAccountsThresholds);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

        }
        protected override UpdateNextAlertThresholdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateNextAlertThresholdsInput()
            {
                AccountTypeId = this.AccountTypeId.Get(context),
                InputQueue = this.InputQueue.Get(context),
            };
        }
    }
}
