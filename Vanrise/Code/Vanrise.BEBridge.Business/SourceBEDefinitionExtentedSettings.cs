using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BEBridge.Business
{
    public class SourceBEDefinitionExtentedSettings : BPDefinitionExtendedSettings
    {
        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {

            return new BEReceiveDefinitionManager().DoesUserHaveViewAccess(context.UserId);
           
        }

        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {

            return new BEReceiveDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);

          
        }

        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {

            return new BEReceiveDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);


        }
        public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            var Args = context.InputArg as SourceBESyncProcessInput;
            return new BEReceiveDefinitionManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, Args.BEReceiveDefinitionIds);
        }
    }
}
