using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetAlertRulesByType<T> : CodeActivity where T : VRAlertRuleCriteria
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<List<VRAlertRule<T>>> AlertRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
