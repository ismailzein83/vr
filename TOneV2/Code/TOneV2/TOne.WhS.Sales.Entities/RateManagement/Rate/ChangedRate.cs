using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ChangedRate
    {
        public long RateId { get; set; }

        public DateTime EED { get; set; }
        public int? RateTypeId { get; set; }
    }
}
