using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetAlertRuleType : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<VRAlertRuleType> AlertRuleType { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();   
        }
    }
}
