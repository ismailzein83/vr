using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleCodeQuery
    {
        public string Code { get; set; }

        public int CodeGroupId { get; set; }

        public int? ZoneId { get; set; }

        public DateTime? EffectiveOn { get; set; }

    }
}
