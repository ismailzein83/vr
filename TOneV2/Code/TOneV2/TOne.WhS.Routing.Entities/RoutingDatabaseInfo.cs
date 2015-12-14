using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingDatabaseInfo
    {
        public int RoutingDatabaseId { get; set; }
        public string Title { get; set; }
        public RoutingDatabaseInformation Information { get; set; }
    }
}
