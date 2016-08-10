using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPBuildCodeMatchesByCodePrefixInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int RoutingDatabaseId { get; set; }

        public string CodePrefix { get; set; }

        public int CodePrefixLength { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle#: {0}", CodePrefix);
        }
    }
}
