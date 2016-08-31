using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingZone : IZone, IExistingEntity
    {
        public BusinessEntity.Entities.SaleZone ZoneEntity { get; set; }

        public ChangedZone ChangedZone { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedZone; }
        }

        public long ZoneId
        {
            get { return ZoneEntity.SaleZoneId; }
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

        List<ExistingCode> _existingCodes = new List<ExistingCode>();
        public List<ExistingCode> ExistingCodes
        {
            get
            {
                return _existingCodes;
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

        List<AddedCode> _addedCodes = new List<AddedCode>();
        public List<AddedCode> AddedCodes
        {
            get
            {
                return _addedCodes;
            }
        }

        public object Key
        {
            get { return this.ZoneId; }
        }

        public string TargetType
        {
            get { return "Zone"; }
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
