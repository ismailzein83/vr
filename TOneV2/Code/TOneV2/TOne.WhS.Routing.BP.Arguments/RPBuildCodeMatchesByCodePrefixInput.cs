using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPBuildCodeMatchesByCodePrefixInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int RoutingDatabaseId { get; set; }

        public CodePrefix CodePrefix { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle#: {0}", CodePrefix.Code);
        }
    }
}
