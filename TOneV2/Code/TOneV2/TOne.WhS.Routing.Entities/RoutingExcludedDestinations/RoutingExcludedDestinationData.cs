using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingExcludedDestinationData
    {
        public List<string> ExcludedCodes { get; set; }

        public List<CodeRange> CodeRanges { get; set; }
    }
}