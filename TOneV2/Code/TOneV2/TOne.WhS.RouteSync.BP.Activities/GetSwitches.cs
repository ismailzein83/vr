using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class GetSwitches : CodeActivity
    {        
        [RequiredArgument]
        public InArgument<List<string>> SwitchIds { get; set; }

        [RequiredArgument]
        public OutArgument<List<SwitchInfo>> Switches { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            List<string> switchIds = this.SwitchIds.Get(context);

            if (switchIds != null)
            {
                List<SwitchInfo> switches = new SwitchManager().GetSwitches(switchIds);
                this.Switches.Set(context, switches);
                context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Switches is done", null);
            }
        }
    }
}
