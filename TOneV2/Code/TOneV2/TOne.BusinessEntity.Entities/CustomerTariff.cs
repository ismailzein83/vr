using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CustomerTariff
    {
        public long TariffID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string Currency { get; set; }
        public decimal CallFee { get; set; }
        public byte FirstPeriod { get; set; }
        public decimal FirstPeriodRate { get; set; }
        public byte FractionUnit { get; set; }
        public DateTime BED { get; set; }
        public DateTime EED { get; set; }
        public string IsEffective { get; set; }
    }
}
