using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ValidateRoutingProcessInput : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingProcessInput> RoutingProcessInput { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            RoutingProcessInput routingProcessInput = this.RoutingProcessInput.Get(context.ActivityContext);

            RoutingProcessMode routingProcessMode = routingProcessInput.RoutingProcessMode;
            List<string> switches = routingProcessInput.Switches;

            if (routingProcessMode == RoutingProcessMode.RouteAnalysis && (switches != null && switches.Count > 0))
                throw new VRBusinessException("Invalid RoutingProcessInput");
        }
    }
}