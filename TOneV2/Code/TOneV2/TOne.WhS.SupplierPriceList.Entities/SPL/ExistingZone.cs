using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{    
    public class ExistingZone : IZone
    {
        public BusinessEntity.Entities.SupplierZone ZoneEntity { get; set; }

        public List<ExistingCode> Codes { get; set; }

        public List<ExistingRate> Rates { get; set; }

        public long ZoneId
        {
            get { return ZoneEntity.SupplierZoneId; }
        }

        public string Name
        {
            get { return ZoneEntity.Name; }
        }

        public DateTime BED
        {
            get { return ZoneEntity.BeginEffectiveDate; }
        }

        public DateTime? EED
        {
            get { return ZoneEntity.EndEffectiveDate; }
        }
    }

    public class ExistingCode
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierCode CodeEntity { get; set; }

        public bool IsImported { get; set; }
    }

    public class ExistingRate
    {
        public ExistingZone ParentZone { get; set; }
        public BusinessEntity.Entities.SupplierRate RateEntity { get; set; }

        public bool IsImported { get; set; }
    }

    public class ExistingZonesByName : Dictionary<string, List<ExistingZone>>
    {

    }

    public class ExistingCodesByCodeValue : Dictionary<string, List<ExistingCode>>
    {

    }

    public class ExistingRatesByZoneName : Dictionary<string, List<ExistingRate>>
    {

    }
}
