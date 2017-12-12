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
    public class CustomerZoneRateHistoryReaderV2
    {
        #region Fields
        private Dictionary<int, SaleEntityRates> _productRatesByProductId;
        private Dictionary<int, SaleEntityRates> _customerRatesByCustomerId;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryReaderV2(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            InitializeFields();
            ReadRates(customerIds, sellingProductIds, zoneIds);
        }
        #endregion

        public IEnumerable<SaleRate> GetProductZoneRates(int sellingProductId, string zoneName)
        {
            SaleEntityRates productRates = _productRatesByProductId.GetRecord(sellingProductId);
            return (productRates != null) ? productRates.GetZoneRates(zoneName) : null;
        }
        public IEnumerable<SaleRate> GetCustomerZoneRates(int customerId, string zoneName)
        {
            SaleEntityRates customerRates = _customerRatesByCustomerId.GetRecord(customerId);
            return (customerRates != null) ? customerRates.GetZoneRates(zoneName) : null;
        }

        #region Private Methods
        private void InitializeFields()
        {
            _productRatesByProductId = new Dictionary<int, SaleEntityRates>();
            _customerRatesByCustomerId = new Dictionary<int, SaleEntityRates>();
        }
        private void ReadRates(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds)
        {
            IEnumerable<SaleRate> rates = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>().GetAllSaleRatesByOwners(sellingProductIds, customerIds, zoneIds);
            StructureRates(rates);
        }
        private void StructureRates(IEnumerable<SaleRate> rates)
        {
            if (rates == null || rates.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();
            var salePriceListManager = new SalePriceListManager();

            foreach (SaleRate rate in rates)
            {
                SalePriceList salePriceList = salePriceListManager.GetPriceList(rate.PriceListId);
                salePriceList.ThrowIfNull("salePriceList");

                Dictionary<int, SaleEntityRates> saleEntityRatesBySaleEntityId = (salePriceList.OwnerType == SalePriceListOwnerType.SellingProduct) ? _productRatesByProductId : _customerRatesByCustomerId;

                SaleEntityRates saleEntityRates = saleEntityRatesBySaleEntityId.GetOrCreateItem(salePriceList.OwnerId, () => { return new SaleEntityRates(); });
                saleEntityRates.AddZoneRate(saleZoneManager.GetSaleZoneName(rate.ZoneId), rate);
            }
        }
        #endregion
    }
}
