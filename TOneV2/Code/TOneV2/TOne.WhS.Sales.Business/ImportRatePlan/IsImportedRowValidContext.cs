using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class IsImportedRowValidContext : IIsImportedRowValidContext
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public ImportedRow ImportedRow { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public SaleZone ExistingZone { get; set; }

        public Dictionary<int, DateTime> CountryBEDsByCountry { get; set; }

        public IEnumerable<int> ClosedCountryIds { get; set; }

        public ImportedRowStatus Status { get; set; }

        public string DateTimeFormat { get; set; }
        public bool AllowRateZero { get; set; }

        public string ErrorMessage { get; set; }

        public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }
        public int LongPrecision { get; set; }
    }

    public class IsValidContext : IIsValidContext
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public IEnumerable<int> AllRateTypeIds { get; set; }

        public ImportedRow ImportedRow { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public SaleZone ExistingZone { get; set; }

        public Dictionary<int, DateTime> CountryBEDsByCountry { get; set; }

        public IEnumerable<int> ClosedCountryIds { get; set; }

        public string DateTimeFormat { get; set; }

        public bool AllowRateZero { get; set; }

        public string ErrorMessage { get; set; }

        public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

        public RateCalculationMethod RateCalculationMethod { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ZoneItem ZoneItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            throw new NotImplementedException();
        }
        public int LongPrecision { get; set; }
    }


    public class IsImportedTargetMatchRowValidContext : IIsImportedCustomerTargetMatchRowValidContext
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public CustomerTargetMatchImportedRow ImportedRow { get; set; }
        public ZoneChanges ZoneDraft { get; set; }
        public SaleZone ExistingZone { get; set; }
        public Dictionary<int, DateTime> CountryBEDsByCountry { get; set; }
        public IEnumerable<int> ClosedCountryIds { get; set; }
        public CustomerTargetMatchImportedRowStatus Status { get; set; }
        public string DateTimeFormat { get; set; }
        public bool AllowRateZero { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }
        public RateCalculationMethod RateCalculationMethod { get; set; }
        public int LongPrecision { get; set; }
        public Func<long, ZoneItem> GetZoneItem { get; set; }
        public Func<Guid, int?> GetCostCalculationMethodIndex { get; set; }
    }

    public class IsTargetMatchValidContext : ICustomerTargetMatchIsValidContext
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public IEnumerable<int> AllRateTypeIds { get; set; }

        public CustomerTargetMatchImportedRow ImportedRow { get; set; }

        public ZoneChanges ZoneDraft { get; set; }

        public SaleZone ExistingZone { get; set; }

        public Dictionary<int, DateTime> CountryBEDsByCountry { get; set; }

        public IEnumerable<int> ClosedCountryIds { get; set; }

        public string DateTimeFormat { get; set; }

        public bool AllowRateZero { get; set; }

        public string ErrorMessage { get; set; }

        //public Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public bool InvalidDueExpectedRateViolation { get; set; }

        public int LongPrecision { get; set; }

        public Func<long, ZoneItem> GetZoneItem { get; set; }

        public Func<Guid, int?> GetCostCalculationMethodIndex { get; set; }
    }

}