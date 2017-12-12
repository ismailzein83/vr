using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityRates
    {
        #region Fields
        private Dictionary<string, List<SaleRate>> _ratesByZoneName;
        #endregion

        #region Constructors
        public SaleEntityRates()
        {
            _ratesByZoneName = new Dictionary<string, List<SaleRate>>();
        }
        #endregion

        public void AddZoneRate(string zoneName, SaleRate rate)
        {
            List<SaleRate> zoneRates = _ratesByZoneName.GetOrCreateItem(GetZoneNameKey(zoneName), () => { return new List<SaleRate>(); });
            zoneRates.Add(rate);
        }
        public IEnumerable<SaleRate> GetZoneRates(string zoneName)
        {
            return _ratesByZoneName.GetRecord(GetZoneNameKey(zoneName));
        }

        #region Private Methods
        private string GetZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
