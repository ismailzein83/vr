using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class SaveDefinitionObjectState : CodeActivity
    {
        public InArgument<string> ObjectKey { get; set; }

        [RequiredArgument]
        public InArgument<object> ObjectValue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();
            ProcessManager processManager = new ProcessManager();
            processManager.SaveDefinitionObjectState(sharedData.InstanceInfo.DefinitionID, this.ObjectKey.Get(context), this.ObjectValue.Get(context));
        }
    }
}
