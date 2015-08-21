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
        public int ZoneId { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
