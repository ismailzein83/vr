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

    public class ExistingCode : Vanrise.Entities.IDateEffectiveSettings
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierCode CodeEntity { get; set; }

        public ChangedCode ChangedCode { get; set; }

        public DateTime BED
        {
            get { return CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedCode != null ? ChangedCode.EED : CodeEntity.EED; }
        }
    }

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
