using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceMarketPrice
    {
        public int SaleZoneMarketPriceID { get; set; }
        public int SaleZoneID { get; set; }
        public short ServicesFlag { get; set; }
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
    }
}
