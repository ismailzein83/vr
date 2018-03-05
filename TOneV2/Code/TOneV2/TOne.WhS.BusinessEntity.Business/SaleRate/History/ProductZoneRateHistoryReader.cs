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
    public class ProductZoneRateHistoryReader
    {
        #region Fields
        private Dictionary<int, SaleEntityRates> _productRatesByProductId;
        #endregion

        #region Constructors
        public ProductZoneRateHistoryReader(IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            InitializeFields();
            ReadRates(sellingProductIds, zoneIds, getNormalRates, getOtherRates);
        }
        #endregion

        public IEnumerable<SaleRate> GetProductZoneRates(int sellingProductId, string zoneName, int? rateTypeId)
        {
            SaleEntityRates productRates = _productRatesByProductId.GetRecord(sellingProductId);
            return (productRates != null) ? productRates.GetZoneRates(zoneName, rateTypeId) : null;
        }

        #region Private Methods
        private void InitializeFields()
        {
            _productRatesByProductId = new Dictionary<int, SaleEntityRates>();
        }
        private void ReadRates(IEnumerable<int> sellingProductIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            IEnumerable<SaleRate> rates = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>().GetAllSaleRatesByOwnerType(SalePriceListOwnerType.SellingProduct, sellingProductIds, zoneIds, getNormalRates, getOtherRates);
            StructureRates(rates);
        }
        private void StructureRates(IEnumerable<SaleRate> rates)
        {
            if (rates == null || rates.Count() == 0)
                return;

            var salePriceListManager = new SalePriceListManager();
            var saleZoneManager = new SaleZoneManager();

            foreach (SaleRate rate in rates.OrderBy(x => x.BED))
            {
                SalePriceList priceList = salePriceListManager.GetPriceList(rate.PriceListId);
                priceList.ThrowIfNull("priceList");

                SaleEntityRates productRates = _productRatesByProductId.GetOrCreateItem(priceList.OwnerId, () => { return new SaleEntityRates(); });

                string zoneName = saleZoneManager.GetSaleZoneName(rate.ZoneId);
                zoneName.ThrowIfNull("zoneName");

                productRates.AddZoneRate(zoneName, rate);
            }
        }
        #endregion
    }
}
