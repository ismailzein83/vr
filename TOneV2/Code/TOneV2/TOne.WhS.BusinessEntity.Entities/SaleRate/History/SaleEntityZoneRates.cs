using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityZoneRates
    {
        #region Fields
        private Dictionary<RateTypeKey, List<SaleRate>> _zoneRatesByType;
        #endregion

        #region Constructors
        public SaleEntityZoneRates()
        {
            _zoneRatesByType = new Dictionary<RateTypeKey, List<SaleRate>>();
        }
        #endregion

        public void AddZoneRate(SaleRate rate)
        {
            List<SaleRate> zoneRatesOfType = _zoneRatesByType.GetOrCreateItem(new RateTypeKey() { RateTypeId = rate.RateTypeId }, () => { return new List<SaleRate>(); });
            zoneRatesOfType.Add(rate);
        }
        public IEnumerable<SaleRate> GetZoneRates(int? rateTypeId)
        {
            return _zoneRatesByType.GetRecord(new RateTypeKey() { RateTypeId = rateTypeId });
        }
    }
}
