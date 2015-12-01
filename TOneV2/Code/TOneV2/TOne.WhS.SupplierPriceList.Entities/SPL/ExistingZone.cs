﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{    
    public class ExistingZone : IZone
    {
        public BusinessEntity.Entities.SupplierZone ZoneEntity { get; set; }

        public List<ExistingCode> ExistingCodes { get; set; }

        List<NewCode> _newCodes = new List<NewCode>();
        public List<NewCode> NewCodes
        {
            get
            {
                return _newCodes;
            }
        }

        public List<ExistingRate> ExistingRates { get; set; }
        
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
            get { return ZoneEntity.BeginEffectiveDate; }
        }

        public DateTime? EED
        {
            get { return ChangedZone != null ? ChangedZone.EED : ZoneEntity.EndEffectiveDate; }
        }
    }

    public class ExistingCode
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierCode CodeEntity { get; set; }

        public bool IsImported { get; set; }

        public ChangedCode ChangedCode { get; set; }

        public DateTime? EED
        {
            get { return ChangedCode != null ? ChangedCode.EED : CodeEntity.EndEffectiveDate; }
        }
    }

    public class ExistingRate
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierRate RateEntity { get; set; }

        public ChangedRate ChangedRate { get; set; }

        public DateTime? EED
        {
            get { return ChangedRate != null ? ChangedRate.EED : RateEntity.EndEffectiveDate; }
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
