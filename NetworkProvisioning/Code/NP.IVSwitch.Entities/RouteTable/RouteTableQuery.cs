using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class RouteTableQuery
    {
        public RouteTableViewType RouteTableViewType { get; set; }
        public string Name { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<int> EndPoints { get; set; }
    }
}
