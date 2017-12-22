using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class NewRate
    {
        public int PriceListId { get; set; }

        public long RateId { get; set; }

        public ExistingZone Zone { get; set; }

        public int? RateTypeId { get; set; }

        public Decimal Rate { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public RateChangeType ChangeType { get; set; }
    }
}
