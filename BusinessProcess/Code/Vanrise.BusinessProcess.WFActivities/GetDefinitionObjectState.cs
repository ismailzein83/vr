using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class GetDefinitionObjectState<T> : CodeActivity<T>
    {
        public InArgument<string> ObjectKey { get; set; }
        
        protected override T Execute(CodeActivityContext context)
        {
            var sharedData = context.GetExtension<BPSharedInstanceData>();
            if (sharedData == null)
                throw new NullReferenceException("BPSharedInstanceData");
            ProcessManager processManager = new ProcessManager();
            return processManager.GetDefinitionObjectState<T>(sharedData.ProcessDefinitionId, this.ObjectKey.Get(context));
        }
    }
}
