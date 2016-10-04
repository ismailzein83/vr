using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleCodeQueryByZone
    {
        public long ZoneId { get; set; }

        public DateTime? EffectiveOn { get; set; }
    }
}
