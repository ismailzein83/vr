using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DraftRateToChange
    {
        //TODO: to remove later if there is no need to, the replacement is zone name
        public long ZoneId { get; set; }

        public int? RateTypeId { get; set; }
        
        public Decimal NormalRate { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
