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
        private Dictionary<string, SaleEntityZoneRates> _ratesByZoneName;
        #endregion

        #region Constructors
        public SaleEntityRates()
        {
            _ratesByZoneName = new Dictionary<string, SaleEntityZoneRates>();
        }
        #endregion

        public void AddZoneRate(string zoneName, SaleRate rate)
        {
            SaleEntityZoneRates zoneRates = _ratesByZoneName.GetOrCreateItem(GetZoneNameKey(zoneName), () => { return new SaleEntityZoneRates(); });
            zoneRates.AddZoneRate(rate);
        }
        public IEnumerable<SaleRate> GetZoneRates(string zoneName, int? rateTypeId)
        {
            SaleEntityZoneRates zoneRates = _ratesByZoneName.GetRecord(GetZoneNameKey(zoneName));
            return (zoneRates != null) ? zoneRates.GetZoneRates(rateTypeId) : null;
        }

        #region Private Methods
        private string GetZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
