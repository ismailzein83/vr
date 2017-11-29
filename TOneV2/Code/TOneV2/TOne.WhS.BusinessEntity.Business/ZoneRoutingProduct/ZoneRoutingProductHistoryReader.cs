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
    public class ZoneRoutingProductHistoryReader
    {
        #region Fields
        private IEnumerable<DefaultRoutingProduct> _sellingProductDefaultRoutingProducts;
        private Dictionary<long, List<SaleZoneRoutingProduct>> _sellingProductZoneRoutingProducts;
        private List<DefaultRoutingProduct> _customerDefaultRoutingProducts;
        private Dictionary<long, List<SaleZoneRoutingProduct>> _customerZoneRoutingProducts;

        private ISaleEntityRoutingProductDataManager _dataManager;
        #endregion

        #region Constructors
        public ZoneRoutingProductHistoryReader(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds)
        {
            InitializeFields();

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                ReadSellingProductData(ownerId, zoneIds);
            else
            {
                int sellingProductId = new CarrierAccountManager().GetSellingProductId(ownerId);
                ReadCustomerData(ownerId, sellingProductId, zoneIds);
            }
        }
        #endregion

        public IEnumerable<DefaultRoutingProduct> GetSellingProductDefaultRoutingProducts()
        {
            return _sellingProductDefaultRoutingProducts;
        }
        public IEnumerable<SaleZoneRoutingProduct> GetSellingProductZoneRoutingProducts(long zoneId)
        {
            return _sellingProductZoneRoutingProducts.GetRecord(zoneId);
        }
        public IEnumerable<DefaultRoutingProduct> GetCustomerDefaultRoutingProducts()
        {
            return _customerDefaultRoutingProducts;
        }
        public IEnumerable<SaleZoneRoutingProduct> GetCustomerZoneRoutingProducts(long zoneId)
        {
            return _customerZoneRoutingProducts.GetRecord(zoneId);
        }

        #region Private Methods
        private void InitializeFields()
        {
            _sellingProductDefaultRoutingProducts = new List<DefaultRoutingProduct>();
            _sellingProductZoneRoutingProducts = new Dictionary<long, List<SaleZoneRoutingProduct>>();
            _customerDefaultRoutingProducts = new List<DefaultRoutingProduct>();
            _customerZoneRoutingProducts = new Dictionary<long, List<SaleZoneRoutingProduct>>();
            _dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
        }
        private void ReadSellingProductData(int sellingProductId, IEnumerable<long> zoneIds)
        {
            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = _dataManager.GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType.SellingProduct, sellingProductId);
            if (defaultRoutingProducts != null)
                _sellingProductDefaultRoutingProducts = defaultRoutingProducts;

            IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = _dataManager.GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneIds);
            if (zoneRoutingProducts != null && zoneRoutingProducts.Count() > 0)
                _sellingProductZoneRoutingProducts = StructureZoneRoutingProducts(zoneRoutingProducts);
        }
        private void ReadCustomerData(int customerId, int sellingProductId, IEnumerable<long> zoneIds)
        {
            var sellingProductDefaultRoutingProducts = new List<DefaultRoutingProduct>();
            var customerDefaultRoutingProducts = new List<DefaultRoutingProduct>();

            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = _dataManager.GetAllDefaultRoutingProducts(sellingProductId, customerId, zoneIds);

            if (defaultRoutingProducts != null && defaultRoutingProducts.Count() > 0)
            {
                foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
                {
                    if (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct)
                        sellingProductDefaultRoutingProducts.Add(defaultRoutingProduct);
                    else
                        customerDefaultRoutingProducts.Add(defaultRoutingProduct);
                }
            }

            _sellingProductDefaultRoutingProducts = sellingProductDefaultRoutingProducts;
            _customerDefaultRoutingProducts = customerDefaultRoutingProducts;

            IEnumerable<SaleZoneRoutingProduct> saleZoneRoutingProducts = _dataManager.GetAllSaleZoneRoutingProducts(sellingProductId, customerId, zoneIds);

            if (saleZoneRoutingProducts != null && saleZoneRoutingProducts.Count() > 0)
            {
                foreach (SaleZoneRoutingProduct saleZoneRoutingProduct in saleZoneRoutingProducts)
                {
                    if (saleZoneRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct)
                        _sellingProductZoneRoutingProducts.GetOrCreateItem(saleZoneRoutingProduct.SaleZoneId, () => { return new List<SaleZoneRoutingProduct>() { saleZoneRoutingProduct }; });
                    else
                        _customerZoneRoutingProducts.GetOrCreateItem(saleZoneRoutingProduct.SaleZoneId, () => { return new List<SaleZoneRoutingProduct>() { saleZoneRoutingProduct }; });
                }
            }
        }
        private Dictionary<long, List<SaleZoneRoutingProduct>> StructureZoneRoutingProducts(IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts)
        {
            var zoneRoutingProductsByZoneId = new Dictionary<long, List<SaleZoneRoutingProduct>>();
            foreach (SaleZoneRoutingProduct zoneRoutingProduct in zoneRoutingProducts)
                zoneRoutingProductsByZoneId.GetOrCreateItem(zoneRoutingProduct.SaleZoneId, () => { return new List<SaleZoneRoutingProduct>() { zoneRoutingProduct }; });
            return zoneRoutingProductsByZoneId;
        }
        #endregion
    }
}
