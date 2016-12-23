using System;
using System.Activities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetVRBalanceAlertRuleTypeSettings : CodeActivity
    {
        public InArgument<Guid> AlertRuleTypeId { get; set; }
        public InOutArgument<VRBalanceAlertRuleTypeSettings> VRBalanceAlertRuleTypeSettings { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            VRAlertRuleTypeManager manager = new VRAlertRuleTypeManager();
            VRBalanceAlertRuleTypeSettings.Set(context, manager.GetVRAlertRuleTypeSettings<VRBalanceAlertRuleTypeSettings>(AlertRuleTypeId.Get(context)));
        }
    }
}
