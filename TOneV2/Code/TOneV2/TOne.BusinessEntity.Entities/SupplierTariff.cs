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
        public bool RepeatFirstPeriod { get; set; }
        public SupplierTariff Clone()
        {
            SupplierTariff copy = new SupplierTariff();
            copy.TariffID = this.TariffID;
            copy.ZoneID = this.ZoneID;
            copy.SupplierID = this.SupplierID;
            copy.CallFee = this.CallFee;
            copy.FirstPeriodRate = this.FirstPeriodRate;
            copy.FirstPeriod = this.FirstPeriod;
            copy.RepeatFirstPeriod = this.RepeatFirstPeriod;
            copy.FractionUnit = this.FractionUnit;
            copy.BeginEffectiveDate = this.BeginEffectiveDate;
            copy.EndEffectiveDate = this.EndEffectiveDate;
            return copy;
        }
    }
}
