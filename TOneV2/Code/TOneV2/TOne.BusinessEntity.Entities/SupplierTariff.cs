using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class SupplierTariff
    {
        public long TariffID { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string CurrencyID { get; set; }
        public decimal CallFee { get; set; }
        public byte FirstPeriod { get; set; }
        public decimal FirstPeriodRate { get; set; }
        public byte FractionUnit { get; set; }
        public DateTime BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string EndEffectiveDateDescription { get; set; }
        public string IsEffective { get; set; }
    }
}
