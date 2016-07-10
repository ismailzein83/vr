using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneRoutingProductToAdd : Vanrise.Entities.IDateEffectiveSettings
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int ZoneRoutingProductId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        private List<NewSaleZoneRoutingProduct> _newSaleZoneRoutingProducts = new List<NewSaleZoneRoutingProduct>();
        public List<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts
        {
            get
            {
                return _newSaleZoneRoutingProducts;
            }
        }

        private List<ExistingSaleZoneRoutingProduct> _changedExistingSaleZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();
        public List<ExistingSaleZoneRoutingProduct> ChangedExistingSaleZoneRoutingProducts
        {
            get
            {
                return _changedExistingSaleZoneRoutingProducts;
            }
        }
    }
}
