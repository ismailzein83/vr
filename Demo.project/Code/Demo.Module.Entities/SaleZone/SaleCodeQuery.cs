using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class SaleCodeQuery
    {
        public List<int> ZonesIds { get; set; }

        public DateTime? EffectiveOn { get; set; }

    }
}
