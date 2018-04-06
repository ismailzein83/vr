using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class TrunkGroup
    {
        public List<CustomerTrunkGroup> CustomerTrunkGroups { get; set; }

        public List<CodeGroupTrunkGroup> CodeGroupTrunkGroups { get; set; }

        public List<TrunkTrunkGroup> TrunkTrunkGroups { get; set; }

        public bool IsBackup { get; set; }
    }

    public class CustomerTrunkGroup
    {
        public int CustomerId { get; set; }  
    }

    public class CodeGroupTrunkGroup
    {
        public string CodeGroup { get; set; }
    }

    public class TrunkTrunkGroup
    {
        public Guid TrunkId { get; set; }

        public int? Percentage { get; set; }

        public int Priority { get; set; }
    }
}