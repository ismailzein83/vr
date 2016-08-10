using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildRoutesByCodePrefixInput : BaseProcessInputArgument
    {
        public int CodePrefixLength { get; set; }

        public int RoutingDatabaseId { get; set; }

        public string CodePrefix { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle#: {0}", CodePrefix);
        }
    }
}
