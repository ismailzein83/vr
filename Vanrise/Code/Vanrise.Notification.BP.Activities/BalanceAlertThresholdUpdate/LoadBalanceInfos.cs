using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate
{
    public class LoadBalanceInfosInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> OutputQueue { get; set; }
    }
    public sealed class LoadBalanceInfos : DependentAsyncActivity<LoadBalanceInfosInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<VREntityBalanceInfoBatch>> OutputQueue { get; set; }

        protected override void DoWork(LoadBalanceInfosInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchCount = 100;
            List<IVREntityBalanceInfo> lstEntityBalanceInfo = new List<IVREntityBalanceInfo>();
            Action<IVREntityBalanceInfo> onBalanceInfoLoaded = (vrEntityBalanceInfo) =>
            {
                if (lstEntityBalanceInfo.Count >= batchCount)
                {
                    inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                    {
                        BalanceInfos = new List<IVREntityBalanceInfo>(lstEntityBalanceInfo)
                    });
                    lstEntityBalanceInfo = new List<IVREntityBalanceInfo>();
                }
                lstEntityBalanceInfo.Add(vrEntityBalanceInfo);
            };
            VRBalanceAlertRuleLoadBalanceInfosContext context = new VRBalanceAlertRuleLoadBalanceInfosContext(onBalanceInfoLoaded);
            context.RuleTypeSettings = inputArgument.RuleTypeSettings;
            inputArgument.RuleTypeSettings.Behavior.LoadBalanceInfos(context);
            if (lstEntityBalanceInfo.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                {
                    BalanceInfos = new List<IVREntityBalanceInfo>(lstEntityBalanceInfo)
                });
            }
        }

        protected override LoadBalanceInfosInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadBalanceInfosInput
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
        public class VRBalanceAlertRuleLoadBalanceInfosContext : IVRBalanceAlertRuleLoadBalanceInfosContext
        {
            Action<IVREntityBalanceInfo> _onBalanceInfoLoaded;

            public VRBalanceAlertRuleLoadBalanceInfosContext(Action<IVREntityBalanceInfo> onBalanceInfoLoaded)
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
