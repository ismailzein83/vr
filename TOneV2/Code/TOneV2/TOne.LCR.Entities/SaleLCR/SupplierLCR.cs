using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
    public class SupplierLCR
    {
        public string SupplierId { get; set; }
        public string ZoneName { get; set; }
        public int ZoneId { get; set; }
        public decimal Rate { get; set; }
        public short ServiceFlag { get; set; }
        public bool IsCodeGroup { get; set; }
        public int PriceListId { get; set; }

    }
}
