using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NotImportedRate
    {
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public bool HasChanged { get; set; }

        public int? RateTypeId { get; set; }

        public decimal Rate { get; set; }

    }
}
