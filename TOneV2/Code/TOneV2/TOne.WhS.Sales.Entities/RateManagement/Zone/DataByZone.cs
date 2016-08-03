using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DataByZone : IRuleTarget
    {
        public RateToChange RateToChange { get; set; }

        public RateToClose RateToClose { get; set; }

        public SaleZoneRoutingProductToAdd SaleZoneRoutingProductToAdd { get; set; }

        public SaleZoneRoutingProductToClose SaleZoneRoutingProductToClose { get; set; }

        public string ZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

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
