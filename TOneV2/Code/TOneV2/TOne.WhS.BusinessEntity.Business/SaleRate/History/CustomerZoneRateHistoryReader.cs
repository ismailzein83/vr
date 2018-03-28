using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRateHistoryReader
    {
        #region Fields
        private Dictionary<int, SaleEntityRates> _productRatesByProductId = new Dictionary<int, SaleEntityRates>();
        private Dictionary<int, SaleEntityRates> _customerRatesByCustomerId = new Dictionary<int, SaleEntityRates>();

        SaleZoneManager _saleZoneManager = new SaleZoneManager();
        SalePriceListManager _salePriceListManager = new SalePriceListManager();
        #endregion

        #region Constructors

        public CustomerZoneRateHistoryReader(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, DateTime BED, DateTime? EED)
        {
            ReadRates(customerIds, sellingProductIds, zoneIds, true, false, BED, EED);
        }

        public CustomerZoneRateHistoryReader(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            ReadRates(customerIds, sellingProductIds, zoneIds, getNormalRates, getOtherRates, null, null);
        }
        public CustomerZoneRateHistoryReader(IEnumerable<SaleRate> productRates, IEnumerable<SaleRate> customerRates)
        {
            StructureRates(productRates, customerRates);
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
        private void ReadRates(IEnumerable<int> customerIds, IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates, DateTime? BED, DateTime? EED)
        {
            IEnumerable<SaleRate> rates = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>().GetAllSaleRatesByOwners(sellingProductIds, customerIds, zoneIds, getNormalRates, getOtherRates, BED, EED);
            StructureRates(rates);
        }
        private void StructureRates(IEnumerable<SaleRate> rates)
        {
            if (rates == null || rates.Count() == 0)
                return;

            foreach (SaleRate rate in rates.OrderBy(x => x.BED))
            {
                SalePriceList salePriceList = _salePriceListManager.GetPriceList(rate.PriceListId);
                salePriceList.ThrowIfNull("salePriceList");

                Dictionary<int, SaleEntityRates> saleEntityRatesBySaleEntityId = (salePriceList.OwnerType == SalePriceListOwnerType.SellingProduct) ? _productRatesByProductId : _customerRatesByCustomerId;
                AddZoneRate(saleEntityRatesBySaleEntityId, salePriceList.OwnerId, rate);
            }
        }
        private void StructureRates(IEnumerable<SaleRate> productRates, IEnumerable<SaleRate> customerRates)
        {
            if (productRates != null && productRates.Count() > 0)
            {
                foreach (SaleRate productRate in productRates.OrderBy(x => x.BED))
                {
                    SalePriceList salePriceList = _salePriceListManager.GetPriceList(productRate.PriceListId);
                    salePriceList.ThrowIfNull("salePriceList");
                    AddZoneRate(_productRatesByProductId, salePriceList.OwnerId, productRate);
                }
            }

            if (customerRates != null && customerRates.Count() > 0)
            {
                foreach (SaleRate customerRate in customerRates.OrderBy(x => x.BED))
                {
                    SalePriceList salePriceList = _salePriceListManager.GetPriceList(customerRate.PriceListId);
                    salePriceList.ThrowIfNull("salePriceList");
                    AddZoneRate(_customerRatesByCustomerId, salePriceList.OwnerId, customerRate);
                }
            }
        }
        private void AddZoneRate(Dictionary<int, SaleEntityRates> saleEntityRatesByEntityId, int saleEntityId, SaleRate saleRate)
        {
            SaleEntityRates saleEntityRates = saleEntityRatesByEntityId.GetOrCreateItem(saleEntityId, () => { return new SaleEntityRates(); });
            saleEntityRates.AddZoneRate(_saleZoneManager.GetSaleZoneName(saleRate.ZoneId), saleRate);
        }
        #endregion
    }
}
