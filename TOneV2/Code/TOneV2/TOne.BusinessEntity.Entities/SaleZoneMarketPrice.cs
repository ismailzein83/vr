using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    /// <summary>
    /// Key ZoneId-ServiceFlag
    /// </summary>
    public class SaleZoneMarketPrices : Dictionary<string, SaleZoneMarketPrice> { 
    
    }
    public class SaleZoneMarketPrice
    {
        public int ZoneId { get; set; }
        public short ServiceFlag { get; set; }
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
    }
}
