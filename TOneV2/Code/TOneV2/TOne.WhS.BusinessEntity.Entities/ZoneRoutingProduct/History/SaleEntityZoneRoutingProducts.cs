using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityZoneRoutingProducts
    {
        #region Fields
        private Dictionary<string, List<SaleZoneRoutingProduct>> _zoneRoutingProductsByZoneName;
        #endregion

        #region Constructors
        public SaleEntityZoneRoutingProducts()
        {
            _zoneRoutingProductsByZoneName = new Dictionary<string, List<SaleZoneRoutingProduct>>();
        }
        #endregion

        public void AddZoneRoutingProduct(string zoneName, SaleZoneRoutingProduct zoneRoutingProduct)
        {
            List<SaleZoneRoutingProduct> zoneRoutingProducts = _zoneRoutingProductsByZoneName.GetOrCreateItem(GetZoneNameKey(zoneName), () => { return new List<SaleZoneRoutingProduct>(); });
            zoneRoutingProducts.Add(zoneRoutingProduct);
        }
        public List<SaleZoneRoutingProduct> GetZoneRoutingProducts(string zoneName)
        {
            return _zoneRoutingProductsByZoneName.GetRecord(GetZoneNameKey(zoneName));
        }

        #region Private Methods
        private string GetZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
