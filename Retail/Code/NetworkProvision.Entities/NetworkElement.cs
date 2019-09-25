using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProvision.Entities
{
    public class NetworkElement
    {
        public long Id { get; set; }
        public Guid TypeId { get; set; }
        public string Name { get; set; }
    }
}
