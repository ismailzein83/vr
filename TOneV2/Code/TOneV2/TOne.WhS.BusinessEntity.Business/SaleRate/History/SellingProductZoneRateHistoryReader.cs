using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SellingProductZoneRateHistoryReader
    {
        #region Fields / Constructors

        private Dictionary<string, SellingProductZoneRates> _ratesByZone;

        public SellingProductZoneRateHistoryReader(int sellingProductId, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            SetRates(sellingProductId, zoneIds, getNormalRates, getOtherRates);
        }

        #endregion

        public IEnumerable<SaleRate> GetSaleRates(string zoneName, int? rateTypeId)
        {
            SellingProductZoneRates spZoneRates = _ratesByZone.GetRecord(zoneName);
            RateTypeKey rateTypeKey = new RateTypeKey() { RateTypeId = rateTypeId };
            return (spZoneRates != null) ? spZoneRates.GetRecord(rateTypeKey) : null;
        }

        #region Private Methods

        private void SetRates(int sellingProductId, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            _ratesByZone = new Dictionary<string, SellingProductZoneRates>();

            IEnumerable<SaleRate> saleRates = new SaleRateManager().GetAllSaleRatesByOwner(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneIds, getNormalRates, getOtherRates);

            if (saleRates == null || saleRates.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();

            foreach (SaleRate saleRate in saleRates.OrderBy(x => x.BED))
            {
                string zoneName = saleZoneManager.GetSaleZoneName(saleRate.ZoneId);

                if (string.IsNullOrWhiteSpace(zoneName))
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of SaleZone '{0}' was not found", saleRate.ZoneId));

                SellingProductZoneRates spZoneRates;

                if (!_ratesByZone.TryGetValue(zoneName, out spZoneRates))
                {
                    spZoneRates = new SellingProductZoneRates();
                    _ratesByZone.Add(zoneName, spZoneRates);
                }

                List<SaleRate> ratesByType;
                var rateTypeKey = new RateTypeKey() { RateTypeId = saleRate.RateTypeId };

                if (!spZoneRates.TryGetValue(rateTypeKey, out ratesByType))
                {
                    ratesByType = new List<SaleRate>();
                    spZoneRates.Add(rateTypeKey, ratesByType);
                }

                ratesByType.Add(saleRate);
            }
        }

        #endregion
    }

    public class SellingProductZoneRates : Dictionary<RateTypeKey, List<SaleRate>>
    {

    }

    public struct RateTypeKey
    {
        public int? RateTypeId { get; set; }
    }
}
