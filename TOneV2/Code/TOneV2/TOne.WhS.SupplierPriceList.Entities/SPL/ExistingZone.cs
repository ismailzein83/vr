using System;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingZone : IZone
    {
        public BusinessEntity.Entities.SupplierZone ZoneEntity { get; set; }

        List<ExistingCode> _existingCodes = new List<ExistingCode>();
        public List<ExistingCode> ExistingCodes
        {
            get
            {
                return _existingCodes;
            }
        }

        List<NewCode> _newCodes = new List<NewCode>();
        public List<NewCode> NewCodes
        {
            get
            {
                return _newCodes;
            }
        }

        List<ExistingRate> _existingRates = new List<ExistingRate>();
        public List<ExistingRate> ExistingRates
        {
            get
            {
                return _existingRates;
            }
        }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }

        public ChangedZone ChangedZone { get; set; }

        public long ZoneId
        {
            get { return ZoneEntity.SupplierZoneId; }
        }

        public string Name
        {
            get { return ZoneEntity.Name; }
        }

        public int CountryId
        {
            get { return ZoneEntity.CountryId; }
        }

        public DateTime BED
        {
            get { return ZoneEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedZone != null ? ChangedZone.EED : ZoneEntity.EED; }
        }
    }

    public class ExistingZonesByName
    {
        private Dictionary<string, List<ExistingZone>> _existingZonesByName;

        public ExistingZonesByName()
        {
            _existingZonesByName = new Dictionary<string, List<ExistingZone>>();
        }
        public void Add(string key, List<ExistingZone> values)
        {
            _existingZonesByName.Add(key.ToLower(), values);
        }

        public bool TryGetValue(string key, out List<ExistingZone> value)
        {
            return _existingZonesByName.TryGetValue(key.ToLower(), out value);
        }

    }

}
