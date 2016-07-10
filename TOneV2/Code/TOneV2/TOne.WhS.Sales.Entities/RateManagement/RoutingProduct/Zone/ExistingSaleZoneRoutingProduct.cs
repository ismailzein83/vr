using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingSaleZoneRoutingProduct : Vanrise.Entities.IDateEffectiveSettings
    {
        public SaleZoneRoutingProduct SaleZoneRoutingProductEntity { get; set; }

        public ChangedSaleZoneRoutingProduct ChangedSaleZoneRoutingProduct { get; set; }

        public DateTime BED
        {
            get { return SaleZoneRoutingProductEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedSaleZoneRoutingProduct != null ? ChangedSaleZoneRoutingProduct.EED : SaleZoneRoutingProductEntity.EED; }
        }
    }

    public class ExistingSaleZoneRoutingProductsByZoneName
    {
        private Dictionary<string, List<ExistingSaleZoneRoutingProduct>> _existingSaleZoneRoutingProductsByZoneName;

        public ExistingSaleZoneRoutingProductsByZoneName()
        {
            _existingSaleZoneRoutingProductsByZoneName = new Dictionary<string, List<ExistingSaleZoneRoutingProduct>>();
        }

        public void Add(string key, List<ExistingSaleZoneRoutingProduct> value)
        {
            _existingSaleZoneRoutingProductsByZoneName.Add(key.ToLower(), value);
        }

        public bool TryGetValue(string key, out List<ExistingSaleZoneRoutingProduct> value)
        {
            return _existingSaleZoneRoutingProductsByZoneName.TryGetValue(key.ToLower(), out value);
        }
    }
}
