using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Tariff
    {
        public int TariffID { get; set; }
        public int ZoneID { get; set; }
        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public decimal CallFee { get; set; }
        public decimal FirstPeriodRate { get; set; }
        public int FirstPeriod { get; set; }
        public string RepeatFirstPeriod { get; set; }
        public int FractionUnit { get; set; }
        public DateTime BeginEffectiveDate { get; set; }
        public Nullable<DateTime> EndEffectiveDate { get; set; }

    }

}
