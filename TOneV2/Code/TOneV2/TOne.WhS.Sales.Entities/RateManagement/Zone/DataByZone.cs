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
        private List<RateToChange> _ratesToChange = new List<RateToChange>();

        public List<RateToChange> RatesToChange
        {
            get 
            { 
                return this._ratesToChange;
            }
        }

        private List<RateToClose> _ratesToClose = new List<RateToClose>();

        public List<RateToClose> RatesToClose 
        {
            get 
            {
                return this._ratesToClose;
            }
        }

        private List<SaleZoneRoutingProductToAdd> _saleZoneRoutingProductsToAdd = new List<SaleZoneRoutingProductToAdd>();

        public List<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd
        {
            get
            {
                return this._saleZoneRoutingProductsToAdd;
            }
        }

        private List<SaleZoneRoutingProductToClose> _saleZoneRoutingProductsToClose = new List<SaleZoneRoutingProductToClose>();

        public List<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose
        {
            get 
            {
                return this._saleZoneRoutingProductsToClose;
            } 
        }

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
