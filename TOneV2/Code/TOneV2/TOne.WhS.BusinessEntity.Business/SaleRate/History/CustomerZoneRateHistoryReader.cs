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
    public class CustomerZoneRateHistoryReader
    {
        #region Fields
        private Dictionary<int, SaleEntityRates> _productRatesByProductId;
        private Dictionary<int, SaleEntityRates> _customerRatesByCustomerId;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryReader(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            InitializeFields();
            ReadRates(customerIds, sellingProductIds, zoneIds, getNormalRates, getOtherRates);
        }
        #endregion

        public IEnumerable<SaleRate> GetProductZoneRates(int sellingProductId, string zoneName, int? rateTypeId)
        {
            SaleEntityRates productRates = _productRatesByProductId.GetRecord(sellingProductId);
            return (productRates != null) ? productRates.GetZoneRates(zoneName, rateTypeId) : null;
        }
        public IEnumerable<SaleRate> GetCustomerZoneRates(int customerId, string zoneName, int? rateTypeId)
        {
            SaleEntityRates customerRates = _customerRatesByCustomerId.GetRecord(customerId);
            return (customerRates != null) ? customerRates.GetZoneRates(zoneName, rateTypeId) : null;
        }

        #region Private Methods
        private void InitializeFields()
        {
            _productRatesByProductId = new Dictionary<int, SaleEntityRates>();
            _customerRatesByCustomerId = new Dictionary<int, SaleEntityRates>();
        }
        private void ReadRates(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            IEnumerable<SaleRate> rates = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>().GetAllSaleRatesByOwners(sellingProductIds, customerIds, zoneIds, getNormalRates, getOtherRates);
            StructureRates(rates);
        }
        private void StructureRates(IEnumerable<SaleRate> rates)
        {
            if (rates == null || rates.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();
            var salePriceListManager = new SalePriceListManager();

            foreach (SaleRate rate in rates.OrderBy(x => x.BED))
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
