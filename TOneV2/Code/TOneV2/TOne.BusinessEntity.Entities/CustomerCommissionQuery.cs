using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CustomerCommissionQuery
    {
        public string CustomerId { get; set; }
        public List<int> ZoneIds { get; set; }
        public DateTime? EffectiveFrom { get; set; }
    }
}
