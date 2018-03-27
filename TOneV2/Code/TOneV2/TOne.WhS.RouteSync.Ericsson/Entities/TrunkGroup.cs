using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class TrunkGroup
    {
        public List<string> TrunkNames { get; set; }

        public List<int> CustomerIds { get; set; }

        public List<string> CodeGroups { get; set; }

        public bool IsBackup { get; set; }
    }
}
