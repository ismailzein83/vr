using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierPriceList
    {
        public int PriceListId { get; set; }

        public int SupplierId { get; set; }

        public int CurrencyId { get; set; }
        public long FileId { get; set; }
    }
}
