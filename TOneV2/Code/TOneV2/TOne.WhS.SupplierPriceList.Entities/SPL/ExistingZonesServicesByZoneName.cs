using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingZonesServicesByZoneName
    {
        private Dictionary<string, List<ExistingZoneService>> _existingZoneServicesByZoneName;

        public ExistingZonesServicesByZoneName()
        {
            this._existingZoneServicesByZoneName = new Dictionary<string, List<ExistingZoneService>>();
            
        }

        public void Add(string key, List<ExistingZoneService> value)
        {
            _existingZoneServicesByZoneName.Add(key.ToLower(), value);
        }

        public bool TryGetValue(string key, out List<ExistingZoneService> value)
        {
            return _existingZoneServicesByZoneName.TryGetValue(key.ToLower(), out value);
        }
    }

}
