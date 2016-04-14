using System;
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

    public class ExistingRatesByZoneName : Dictionary<string, List<ExistingRate>>
    {

    }
}
