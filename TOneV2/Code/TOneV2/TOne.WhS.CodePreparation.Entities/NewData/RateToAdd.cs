using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class RateToAdd
    {
        public string ZoneName { get; set; }

        public PriceListToAdd PriceListToAdd { get; set; }

        public decimal Rate { get; set; }
        public int? CurrencyId { get; set; }

        private List<AddedRate> _addedRate = new List<AddedRate>();

        public List<AddedRate> AddedRates
        {
            get
            {
                return this._addedRate;
            }
        }
    }

    public class ZoneRoutingProductToAdd
    {
        public string ZoneName { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public int RoutingProductId { get; set; }

        private List<AddedZoneRoutingProduct> _addedZonesRoutingProducts = new List<AddedZoneRoutingProduct>();

        public List<AddedZoneRoutingProduct> AddedZonesRoutingProducts
        {
            get
            {
                return this._addedZonesRoutingProducts;
            }
        }
    }

    public class AddedZoneRoutingProduct
    {
        public long SaleEntityRoutingProductId { get; set; }
        public AddedZone AddedZone { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int  OwnerId { get; set; }
        public int RoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        
    }
}
