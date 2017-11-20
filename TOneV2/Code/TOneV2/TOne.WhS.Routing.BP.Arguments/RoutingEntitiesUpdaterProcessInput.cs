using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RoutingEntitiesUpdaterProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle#");
        }
    }
}