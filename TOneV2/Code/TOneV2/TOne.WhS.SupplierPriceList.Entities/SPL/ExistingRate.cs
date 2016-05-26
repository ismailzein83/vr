﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingRate : Vanrise.Entities.IDateEffectiveSettings
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierRate RateEntity { get; set; }

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
         private Dictionary<string, List<ExistingRate>> _ExistingRatesByZoneName;

        public ExistingRatesByZoneName()
        {
            _ExistingRatesByZoneName = new Dictionary<string, List<ExistingRate>>();
        }
        public void Add(string key, List<ExistingRate> values)
        {
            _ExistingRatesByZoneName.Add(key.ToLower(), values);
        }

        public bool TryGetValue(string key, out List<ExistingRate> value)
        {
            value= new List<ExistingRate>();
            return _ExistingRatesByZoneName.TryGetValue(key.ToLower(), out value);
        }

    }
    
}
