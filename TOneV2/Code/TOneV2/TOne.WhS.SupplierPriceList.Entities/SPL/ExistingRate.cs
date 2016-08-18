using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingRate : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierRate RateEntity { get; set; }

        public ChangedRate ChangedRate { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedRate; }
        }

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
            value= new List<ExistingRate>();
            return _existingRatesByZoneName.TryGetValue(key.ToLower(), out value);
        }

    }
    
}
