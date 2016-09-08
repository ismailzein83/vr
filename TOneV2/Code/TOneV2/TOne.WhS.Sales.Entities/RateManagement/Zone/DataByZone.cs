using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DataByZone : IRuleTarget
    {
        public string ZoneName { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public DateTime? SoldOn { get; set; }
        public ZoneRateGroup ZoneRateGroup { get; set; }
        public RateToChange NormalRateToChange { get; set; }
        public List<RateToChange> OtherRatesToChange { get; set; }
        public RateToClose NormalRateToClose { get; set; }
        public List<RateToClose> OtherRatesToClose { get; set; }
        public SaleZoneRoutingProductToAdd SaleZoneRoutingProductToAdd { get; set; }
        public SaleZoneRoutingProductToClose SaleZoneRoutingProductToClose { get; set; }
        public SaleZoneServiceToAdd SaleZoneServiceToAdd { get; set; }
        public SaleZoneServiceToClose SaleZoneServiceToClose { get; set; }

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.ZoneName; }
        }

        public string TargetType
        {
            get { return "Zone"; }
        }

        #endregion
    }

    public class ZoneRateGroup
    {
        public ZoneRate NormalRate { get; set; }
        public Dictionary<int, ZoneRate> OtherRatesByType { get; set; }
    }

    public class ZoneRate
    {
        public SalePriceListOwnerType Source { get; set; }
        public int? RateTypeId { get; set; }
        public decimal Rate { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
