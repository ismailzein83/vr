using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SupplierTargetMatchFilter
    {
        public int SellingNumberPlanId { get; set; }
        public int RoutingProductId { get; set; }
        public int RoutingDataBaseId { get; set; }
        public IEnumerable<int> CountryIds { get; set; }
        public Guid PolicyId { get; set; }
        public int NumberOfOptions { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class SupplierTargetSettings
    {
        public TargetMatchCalculationMethod CalculationMethod { get; set; }
        public MarginType MarginType { get; set; }
        public decimal MarginValue { get; set; }
        public decimal DefaultVolume { get; set; }
        public decimal DefaultASR { get; set; }
        public decimal DefaultACD { get; set; }
        public bool IncludeACD_ASR { get; set; }
        public decimal VolumeMultiplier { get; set; }
    }

    public enum MarginType
    {
        Fixed,
        Percentage
    }


}
