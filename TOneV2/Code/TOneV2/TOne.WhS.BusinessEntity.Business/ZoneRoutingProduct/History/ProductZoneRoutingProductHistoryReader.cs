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
    public class ProductZoneRoutingProductHistoryReader
    {
        #region Fields
        private Dictionary<int, SaleEntityRoutingProducts> _productRoutingProducts;
        #endregion

        #region Constructors
        public ProductZoneRoutingProductHistoryReader(IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            InitializeFields();
            ReadRoutingProducts(sellingProductIds, zoneIds);
        }
        #endregion

        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(int sellingProductId)
        {
            SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetRecord(sellingProductId);
            return (productRoutingProducts != null) ? productRoutingProducts.GetDefaultRoutingProducts() : null;
        }
        public IEnumerable<SaleZoneRoutingProduct> GetZoneRoutingProducts(int sellingProductId, string zoneName)
        {
            SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetRecord(sellingProductId);
            return (productRoutingProducts != null) ? productRoutingProducts.GetZoneRoutingProducts(zoneName) : null;
        }

        #region Private Methods
        private void InitializeFields()
        {
            _productRoutingProducts = new Dictionary<int, SaleEntityRoutingProducts>();
        }
        private void ReadRoutingProducts(IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = dataManager.GetAllDefaultRoutingProductsByOwners(SalePriceListOwnerType.SellingProduct, sellingProductIds);
            StructureDefaultRoutingProducts(defaultRoutingProducts);

            IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = dataManager.GetAllZoneRoutingProductsByOwners(SalePriceListOwnerType.SellingProduct, sellingProductIds, zoneIds);
            StructureZoneRoutingProducts(zoneRoutingProducts);
        }
        private void StructureDefaultRoutingProducts(IEnumerable<DefaultRoutingProduct> defaultRoutingProducts)
        {
            if (defaultRoutingProducts == null || defaultRoutingProducts.Count() == 0)
                return;

            foreach (DefaultRoutingProduct defaultRoutingPrdouct in defaultRoutingProducts)
            {
                SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetOrCreateItem(defaultRoutingPrdouct.OwnerId, () => { return new SaleEntityRoutingProducts(); });
                productRoutingProducts.AddDefaultRoutingProduct(defaultRoutingPrdouct);
            }
        }
        private void StructureZoneRoutingProducts(IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts)
        {
            if (zoneRoutingProducts == null || zoneRoutingProducts.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();

            foreach (SaleZoneRoutingProduct zoneRoutingProduct in zoneRoutingProducts)
            {
                SaleEntityRoutingProducts productRoutingProducts = _productRoutingProducts.GetOrCreateItem(zoneRoutingProduct.OwnerId, () => { return new SaleEntityRoutingProducts(); });
                productRoutingProducts.AddZoneRoutingProduct(saleZoneManager.GetSaleZoneName(zoneRoutingProduct.SaleZoneId), zoneRoutingProduct);
            }
        }
        #endregion
    }
}
