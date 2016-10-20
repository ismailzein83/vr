using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceQuery
    {
        public string Name { get; set; }

        public List<Guid> AdapterTypeIDs { get; set; }

        public bool? IsEnabled { get; set; }
    }
}
