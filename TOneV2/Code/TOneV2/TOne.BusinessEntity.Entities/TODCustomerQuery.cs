using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class TODCustomerQuery
    {
        public string CustomerId { get; set; }
        public List<int> ZoneIds { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
