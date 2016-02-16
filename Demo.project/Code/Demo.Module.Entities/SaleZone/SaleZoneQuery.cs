using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class SaleZoneQuery
    {
        public List<int> Countries { get; set; }
        public string Name { get; set; }
        public DateTime? EffectiveOn { get; set; }
    }
}
