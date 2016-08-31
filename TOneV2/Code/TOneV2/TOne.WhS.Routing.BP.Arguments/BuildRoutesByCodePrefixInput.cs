using System;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildRoutesByCodePrefixInput : BaseProcessInputArgument
    {
        public int SupplierCodeServiceRuntimeProcessId { get; set; }

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