using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
  public  class SaleRate
    {
        public int ZoneId { get; set; }
        public string CodeGroup { get; set; }
        public string ZoneName { get; set; }
        public FlaggedService OurServicesFlag { get; set; }
        public int PriceListId { get; set; }
        public Currency Currency { get; set; }
        public decimal Rate { get; set; }
        public List<string> EffectiveCodes { get; set; }
        public List<SupplierZoneRate> SupplierZoneRates { get; set; }
    }
}
