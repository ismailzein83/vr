using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
    public class CustomerSaleZone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public short ServiceFlag { get; set; }
        public int PriceListId { get; set; }
        public decimal Rate { get; set; }
        public List<SupplierLCR> SuppliersLcr { get; set; }
    }
}
