using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetAlertRuleTypeSettings<T> : CodeActivity where T : VRAlertRuleTypeSettings
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<T> AlertRuleTypeSettings { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            
        }
    }
}
