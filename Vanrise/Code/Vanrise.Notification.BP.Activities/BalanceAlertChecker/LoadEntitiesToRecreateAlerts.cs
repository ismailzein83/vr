using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class LoadEntitiesToRecreateAlertInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }

        public BaseQueue<VREntityBalanceInfoBatch> OutputQueue { get; set; }
    }

    public sealed class LoadEntitiesToRecreateAlerts : DependentAsyncActivity<LoadEntitiesToAlertInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<VREntityBalanceInfoBatch>> OutputQueue { get; set; }


        protected override void DoWork(LoadEntitiesToAlertInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Account Balances for Recreate Alerts.");
            int batchCount = 100;
            List<IVREntityBalanceInfo> lstBalanceInfoToAlert = new List<IVREntityBalanceInfo>();
            Action<IVREntityBalanceInfo> onBalanceInfoLoaded = (vrEntityBalanceInfo) =>
            {
                if (lstBalanceInfoToAlert.Count >= batchCount)
                {
                    inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                    {
                        BalanceInfos = new List<IVREntityBalanceInfo>(lstBalanceInfoToAlert)
                    });
                    lstBalanceInfoToAlert = new List<IVREntityBalanceInfo>();
                }
                lstBalanceInfoToAlert.Add(vrEntityBalanceInfo);
            };
            VRBalanceAlertRuleLoadEntitiesToRecreateAlertContext context = new VRBalanceAlertRuleLoadEntitiesToRecreateAlertContext(onBalanceInfoLoaded);
            context.RuleTypeSettings = inputArgument.RuleTypeSettings;
            inputArgument.RuleTypeSettings.Behavior.LoadEntitiesToRecreateAlerts(context);
            if (lstBalanceInfoToAlert.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                {
                    BalanceInfos = new List<IVREntityBalanceInfo>(lstBalanceInfoToAlert)
                });
            }
        }

        protected override LoadEntitiesToAlertInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadEntitiesToAlertInput
            {
                RuleTypeSettings = this.RuleTypeSettings.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<VREntityBalanceInfoBatch>());
            base.OnBeforeExecute(context, handle);
        }

        #region Contexts Implementation
        public class VRBalanceAlertRuleLoadEntitiesToRecreateAlertContext : IVRBalanceAlertRuleLoadEntitiesToRecreateAlertContext
        {
            Action<IVREntityBalanceInfo> _onBalanceInfoLoaded;
            public VRBalanceAlertRuleLoadEntitiesToRecreateAlertContext(Action<IVREntityBalanceInfo> onBalanceInfoLoaded)
            {
                _onBalanceInfoLoaded = onBalanceInfoLoaded;
            }
            public void OnBalanceInfoLoaded(IVREntityBalanceInfo balanceInfo)
            {
                _onBalanceInfoLoaded(balanceInfo);
            }

            public VRBalanceAlertRuleTypeSettings RuleTypeSettings
            {
                get;
                set;
            }
        }

        #endregion
    }
}
