using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public ExistingZone ParentZone { get; set; }

        public decimal ConvertedRate { get; set; }

        public BusinessEntity.Entities.SaleRate RateEntity { get; set; }

        public ChangedRate ChangedRate { get; set; }

        public DateTime BED
        {
            get { return RateEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedRate != null ? ChangedRate.EED : RateEntity.EED; }
        }
    }

    public class ExistingRatesByZoneName
    {
        private Dictionary<string, List<ExistingRate>> _existingRatesByZoneName;

        public ExistingRatesByZoneName()
        {
            _existingRatesByZoneName = new Dictionary<string, List<ExistingRate>>();
        }
        public void Add(string key, List<ExistingRate> values)
        {
            _existingRatesByZoneName.Add(key.ToLower(), values);
        }

        public bool TryGetValue(string key, out List<ExistingRate> value)
        {
            value = new List<ExistingRate>();
            return _existingRatesByZoneName.TryGetValue(key.ToLower(), out value);
        }

    }
}
