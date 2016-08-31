using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingRateGroup
    {
        public string ZoneName { get; set; }

        private List<ExistingRate> _normalRates = new List<ExistingRate>();
        public List<ExistingRate> NormalRates
        {
            get
            {
                return this._normalRates;
            }
        }

        private Dictionary<int, List<ExistingRate>> _otherRates = new Dictionary<int, List<ExistingRate>>();
        public Dictionary<int, List<ExistingRate>> OtherRates
        {
            get
            {
                return this._otherRates;
            }
        }

    }

}
