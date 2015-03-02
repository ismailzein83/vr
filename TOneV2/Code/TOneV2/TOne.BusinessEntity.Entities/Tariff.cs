using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Tariff : IZoneSupplied
    {
        public int TariffID { get; set; }
        public int ZoneId { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public decimal CallFee { get; set; }
        public decimal FirstPeriodRate { get; set; }
        public int FirstPeriod { get; set; }
        public bool RepeatFirstPeriod { get; set; }
        public int FractionUnit { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }

        public Tariff Clone()
        {
            Tariff copy = new Tariff();
            copy.TariffID = this.TariffID;
            copy.ZoneId = this.ZoneId;
            copy.SupplierId = this.SupplierId;
            copy.CustomerId = this.CustomerId;
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
