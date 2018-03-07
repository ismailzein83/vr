using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class PartialRoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int? RouteRuleId { get; set; }
        
        public int? RouteOptionRuleId { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle#");
        }
    }
}