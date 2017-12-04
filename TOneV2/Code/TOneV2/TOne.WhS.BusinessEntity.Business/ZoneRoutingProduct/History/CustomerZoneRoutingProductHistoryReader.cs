using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRoutingProductHistoryReader
    {
        #region Fields
        private Dictionary<int, SaleEntityRoutingProducts> _productRoutingProducts;
        private Dictionary<int, SaleEntityRoutingProducts> _customerRoutingProducts;
        #endregion

        #region Constructors
        public CustomerZoneRoutingProductHistoryReader(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            InitializeFields();
            ReadRoutingProducts(customerIds, sellingProductIds, zoneIds);
        }
        #endregion

        public IEnumerable<DefaultRoutingProduct> GetProductDefaultRoutingProducts(int sellingProductId)
        {
            SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetRecord(sellingProductId);
            return (productRoutingProducts != null) ? productRoutingProducts.GetDefaultRoutingProducts() : null;
        }
        public IEnumerable<SaleZoneRoutingProduct> GetProductZoneRoutingProducts(int sellingProductId, string zoneName)
        {
            SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetRecord(sellingProductId);
            return (productRoutingProducts != null) ? productRoutingProducts.GetZoneRoutingProducts(zoneName) : null;
        }
        public IEnumerable<DefaultRoutingProduct> GetCustomerDefaultRoutingProducts(int customerId)
        {
            SaleEntityRoutingProducts customerRoutingProducts = _customerRoutingProducts.GetRecord(customerId);
            return (customerRoutingProducts != null) ? customerRoutingProducts.GetDefaultRoutingProducts() : null;
        }
        public IEnumerable<SaleZoneRoutingProduct> GetCustomerZoneRoutingProducts(int customerId, string zoneName)
        {
            SaleEntityRoutingProducts customerRoutingProducts = _customerRoutingProducts.GetRecord(customerId);
            return (customerRoutingProducts != null) ? customerRoutingProducts.GetZoneRoutingProducts(zoneName) : null;
        }

        #region Private Methods
        private void InitializeFields()
        {
            _productRoutingProducts = new Dictionary<int, SaleEntityRoutingProducts>();
            _customerRoutingProducts = new Dictionary<int, SaleEntityRoutingProducts>();
        }
        private void ReadRoutingProducts(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = dataManager.GetAllDefaultRoutingProductsByOwners(sellingProductIds, customerIds);
            StructureDefaultRoutingProducts(defaultRoutingProducts);

            IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = dataManager.GetAllZoneRoutingProductsByOwners(sellingProductIds, customerIds, zoneIds);
            StructureZoneRoutingProducts(zoneRoutingProducts);
        }
        private void StructureDefaultRoutingProducts(IEnumerable<DefaultRoutingProduct> defaultRoutingProducts)
        {
            if (defaultRoutingProducts == null || defaultRoutingProducts.Count() == 0)
                return;

            foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts.OrderBy(x => x.BED))
            {
                Dictionary<int, SaleEntityRoutingProducts> saleEntityRoutingProductsBySaleEntityId = (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? _productRoutingProducts : _customerRoutingProducts;
                SaleEntityRoutingProducts saleEntityRoutingProducts = saleEntityRoutingProductsBySaleEntityId.GetOrCreateItem(defaultRoutingProduct.OwnerId, () => { return new SaleEntityRoutingProducts(); });
                saleEntityRoutingProducts.AddDefaultRoutingProduct(defaultRoutingProduct);
            }
        }
        private void StructureZoneRoutingProducts(IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts)
        {
            if (zoneRoutingProducts == null || zoneRoutingProducts.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();

            foreach (SaleZoneRoutingProduct zoneRoutingProduct in zoneRoutingProducts.OrderBy(x => x.BED))
            {
                Dictionary<int, SaleEntityRoutingProducts> saleEntityRoutingProductsBySaleEntityId = (zoneRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct) ? _productRoutingProducts : _customerRoutingProducts;
                SaleEntityRoutingProducts saleEntityRoutingProducts = saleEntityRoutingProductsBySaleEntityId.GetOrCreateItem(zoneRoutingProduct.OwnerId, () => { return new SaleEntityRoutingProducts(); });
                saleEntityRoutingProducts.AddZoneRoutingProduct(saleZoneManager.GetSaleZoneName(zoneRoutingProduct.SaleZoneId), zoneRoutingProduct);
            }
        }
        #endregion
    }
}
