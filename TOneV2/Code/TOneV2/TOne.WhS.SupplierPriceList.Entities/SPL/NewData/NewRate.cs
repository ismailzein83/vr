using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NewRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public long RateId { get; set; }

        public IZone Zone { get; set; }

        public Decimal NormalRate { get; set; }

        public int? RateTypeId { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        public RateChangeType Change { get; set; }
        public bool IsExcluded { get; set; }
    }
}
