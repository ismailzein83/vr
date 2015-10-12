using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
    public class SupplierZoneRate
    {
        public string SupplierId { get; set; }
        public string ZoneName { get; set; }
        public int ZoneId { get; set; }
        public decimal Rate { get; set; }
        public FlaggedService SupplierServicesFlag { get; set; }
        public bool IsCodeGroup { get; set; }
        public int PriceListId { get; set; }
        public string SupplierName { get; set; }
        public Currency Currency { get; set; }

    }
}
