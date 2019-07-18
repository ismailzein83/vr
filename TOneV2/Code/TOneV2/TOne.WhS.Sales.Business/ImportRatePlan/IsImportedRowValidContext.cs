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

        public bool InvalidDueExpectedRateViolation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            throw new NotImplementedException();
        }
    }


    public class IsImportedTargetMatchRowValidContext : IIsImportedTargetMatchRowValidContext
    {
        public IEnumerable<CostCalculationMethod> CostCalculationMethods { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public ZoneItem ZoneItem { get; set; }
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
        public RateCalculationMethod RateCalculationMethod { get; set; }
    }

    public class IsTargetMatchValidContext : IIsValidContext
    {
        private IEnumerable<CostCalculationMethod> _costCalculationMethods;
        public IsTargetMatchValidContext(IEnumerable<CostCalculationMethod> costCalculationMethods)
        {
            _costCalculationMethods = costCalculationMethods;
        }
        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId)
        {
            return UtilitiesManager.GetCostCalculationMethodIndex(_costCalculationMethods, costCalculationMethodConfigId);
        }

        public ZoneItem ZoneItem { get; set; }

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

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public bool InvalidDueExpectedRateViolation { get; set; }
    }

}