using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityRoutingProducts
    {
        #region Fields
        private List<DefaultRoutingProduct> _defaultRoutingProducts;
        private SaleEntityZoneRoutingProducts _zoneRoutingProducts;
        #endregion

        #region Constructors
        public SaleEntityRoutingProducts()
        {
            _defaultRoutingProducts = new List<DefaultRoutingProduct>();
            _zoneRoutingProducts = new SaleEntityZoneRoutingProducts();
        }
        #endregion

        public void AddDefaultRoutingProduct(DefaultRoutingProduct defaultRoutingProduct)
        {
            _defaultRoutingProducts.Add(defaultRoutingProduct);
        }
        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts()
        {
            return _defaultRoutingProducts;
        }

        public void AddZoneRoutingProduct(string zoneName, SaleZoneRoutingProduct zoneRoutingProduct)
        {
            _zoneRoutingProducts.AddZoneRoutingProduct(GetZoneNameKey(zoneName), zoneRoutingProduct);
        }
        public List<SaleZoneRoutingProduct> GetZoneRoutingProducts(string zoneName)
        {
            return _zoneRoutingProducts.GetZoneRoutingProducts(zoneName);
        }

        #region Private Methods
        private string GetZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
