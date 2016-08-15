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
        public SaleEntityZoneRate CurrentRate { get; set; }
        public RateToChange NormalRateToChange { get; set; }
        public List<RateToChange> OtherRatesToChange { get; set; }
        public RateToClose NormalRateToClose { get; set; }
        public List<RateToClose> OtherRatesToClose { get; set; }
        public SaleZoneRoutingProductToAdd SaleZoneRoutingProductToAdd { get; set; }
        public SaleZoneRoutingProductToClose SaleZoneRoutingProductToClose { get; set; }

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
}
