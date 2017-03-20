using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction
{
    public class WFShcedulerTaskActionExtendedSettings : ActionTypeExtendedSettings
    {
        public override bool DoesUserHaveViewAccess(IActionTypeDoesUserHaveViewAccessContext context)
        {
            return new BPDefinitionManager().DoesUserHaveViewAccessInSchedule(context.UserId);
        }

        public override bool DoesUserHaveViewSpecificTaskAccess(IActionTypeDoesUserHaveViewSpecificInstanceAccessContext context)
        {
            WFTaskActionArgument wfTaskActionArgument = context.TaskActionArgument as WFTaskActionArgument;
            return new BPDefinitionManager().DoesUserHaveViewAccess(wfTaskActionArgument.ProcessInputArguments.ProcessName);
        }
        public override bool DoesUserHaveConfigureTaskAccess(IActionTypeDoesUserHaveConfigureInstanceAccessContext context)
        {
            return new BPDefinitionManager().DoesUserHaveScheduleTaskAccess(context.UserId);
        }
        public override bool DoesUserHaveConfigureSpecificTaskAccess(IActionTypeDoesUserHaveConfigureSpecificInstanceAccessContext context)
        {
            WFTaskActionArgument wfTaskActionArgument = context.TaskActionArgument as WFTaskActionArgument;
            return new BPDefinitionManager().DoesUserHaveScheduleSpecificTaskAccess(context.DefinitionContext.UserId,wfTaskActionArgument.ProcessInputArguments);
        }

        public override bool DoesUserHaveRunAccess(IActionTypeDoesUserHaveRunAccessContext context)
        {
            return new BPDefinitionManager().DoesUserHaveScheduleTaskAccess(context.UserId);
        }

        public override bool DoesUserHaveRunSpecificTaskAccess(IActionTypeDoesUserHaveRunSpecificInstanceAccessContext context)
        {

            WFTaskActionArgument wfTaskActionArgument = context.TaskActionArgument as WFTaskActionArgument;
            return new BPDefinitionManager().DoesUserHaveScheduleSpecificTaskAccess(context.DefinitionContext.UserId, wfTaskActionArgument.ProcessInputArguments);
        }
    }
}
