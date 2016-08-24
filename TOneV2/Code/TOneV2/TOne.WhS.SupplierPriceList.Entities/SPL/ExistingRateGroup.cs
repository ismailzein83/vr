using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingRateGroup 
    {
        public ExistingRateGroup()
        {
            NormalRates = new List<ExistingRate>();
            OtherRates = new Dictionary<int, List<ExistingRate>>();
        }
        public string ZoneName { get; set; }
        public List<ExistingRate> NormalRates { get; set; }
        public Dictionary<int,List<ExistingRate>> OtherRates { get; set; }

    }

}
