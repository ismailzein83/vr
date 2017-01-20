using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess
{
    public abstract class BaseCodeActivity : CodeActivity
    {
        protected override sealed void Execute(CodeActivityContext context)
        {
            Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(context.GetSharedInstanceData().InstanceInfo.InitiatorUserId);
            VRExecute(new BaseCodeActivityContext { ActivityContext = context });
        }

        protected abstract void VRExecute(IBaseCodeActivityContext context);

        private class BaseCodeActivityContext : IBaseCodeActivityContext
        {
            public CodeActivityContext ActivityContext
            {
                get;
                set;
            }
        }
    }

    public interface IBaseCodeActivityContext
    {
        CodeActivityContext ActivityContext { get; }
    }

    
}
