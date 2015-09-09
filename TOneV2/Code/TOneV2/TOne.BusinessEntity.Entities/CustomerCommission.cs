using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
   public class CustomerCommission
    {
        public int ID { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public int ZoneId { get; set; }
        public float? FromRate { get; set; }
        public float? ToRate { get; set; }
        public float? Percentage { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public bool IsExtraCharge { get; set; }
        public bool IsEffective { get; set; }
        public int UserId { get; set; }
        public string ZoneName { get; set; }
        public string CustomerName { get; set; }
        public string Currency { get; set; }
        public CustomerCommission Clone()
        {
            CustomerCommission copy = new CustomerCommission();
            copy.ID = this.ID;
            copy.SupplierId = this.SupplierId;
            copy.CustomerId = this.CustomerId;
            copy.ZoneId = this.ZoneId;
            copy.FromRate = this.FromRate;
            copy.ToRate = this.ToRate;
            copy.Percentage = this.Percentage;
            copy.Amount = this.Amount;
            copy.BeginEffectiveDate = this.BeginEffectiveDate;
            copy.EndEffectiveDate = this.EndEffectiveDate;
            copy.IsExtraCharge = this.IsExtraCharge;
            copy.IsEffective = this.IsEffective;
            copy.UserId = this.UserId;
            return copy;
        }
        public double DeductedRateValue(double value)
        {
            double deductedValue = value;
            double factor =  -1;

            // Percentage?
            if (this.Percentage.HasValue && this.Percentage.Value > 0)
                deductedValue = value * (1 + factor * this.Percentage.Value / 100.0);
            else // Fixed amount
                deductedValue = value + factor * (double)this.Amount.Value;
            return deductedValue;
        }
    }
}
