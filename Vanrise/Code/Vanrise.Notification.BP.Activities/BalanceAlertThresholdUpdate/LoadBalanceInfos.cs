using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

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
            long totalBalanceEntitiesCount = 0;
            int totalBatchesCount = 0;
            List<IVREntityBalanceInfo> lstEntityBalanceInfo = new List<IVREntityBalanceInfo>();
            Action<IVREntityBalanceInfo> onBalanceInfoLoaded = (vrEntityBalanceInfo) =>
            {
                totalBalanceEntitiesCount++;
                if (lstEntityBalanceInfo.Count >= batchCount)
                {
                    inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                    {
                        BalanceInfos = new List<IVREntityBalanceInfo>(lstEntityBalanceInfo)
                    });
                    lstEntityBalanceInfo = new List<IVREntityBalanceInfo>();
                    totalBatchesCount++;
                }
                lstEntityBalanceInfo.Add(vrEntityBalanceInfo);
            };
            VRBalanceAlertRuleLoadBalanceInfosContext context = new VRBalanceAlertRuleLoadBalanceInfosContext(onBalanceInfoLoaded);
            context.RuleTypeSettings = inputArgument.RuleTypeSettings;
            inputArgument.RuleTypeSettings.Behavior.LoadBalanceInfos(context);
            if (lstEntityBalanceInfo.Count > 0)
            {
                totalBatchesCount++;
                inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                {
                    BalanceInfos = new List<IVREntityBalanceInfo>(lstEntityBalanceInfo)
                });
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} Total Balance Entity Loaded.", totalBalanceEntitiesCount);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} Balance Entity Batches Created.", totalBatchesCount);
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
