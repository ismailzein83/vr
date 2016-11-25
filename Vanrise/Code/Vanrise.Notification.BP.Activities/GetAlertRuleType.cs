using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;

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
