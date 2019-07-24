using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface IImportedRowValidator
    {
        bool IsValid(IIsValidContext context);
    }

    public interface IIsValidContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        IEnumerable<int> AllRateTypeIds { get; }

        ImportedRow ImportedRow { get; }

        ZoneChanges ZoneDraft { get; }

        SaleZone ExistingZone { get; }

        Dictionary<int, DateTime> CountryBEDsByCountry { get; }

        IEnumerable<int> ClosedCountryIds { get; }

        string DateTimeFormat { get; }

        string ErrorMessage { set; }

        Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

        bool AllowRateZero { get; }

        int LongPrecision { get; set; }
    }

    public interface ICustomerTargetMatchImportedRowValidator
    {
        bool IsValid(ICustomerTargetMatchIsValidContext context);
    }
    public interface ICustomerTargetMatchIsValidContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        IEnumerable<int> AllRateTypeIds { get; }

        CustomerTargetMatchImportedRow ImportedRow { get; }

        ZoneChanges ZoneDraft { get; }

        SaleZone ExistingZone { get; }

        Dictionary<int, DateTime> CountryBEDsByCountry { get; }

        IEnumerable<int> ClosedCountryIds { get; }

        string DateTimeFormat { get; }

        string ErrorMessage { set; }

        bool AllowRateZero { get; }

        bool InvalidDueExpectedRateViolation { get; set; }

        int LongPrecision { get; set; }

        Func<long, ZoneItem> GetZoneItem { get; set; }

        Func<Guid, int?> GetCostCalculationMethodIndex { get; set; }

        RateCalculationMethod RateCalculationMethod { get; set; }
    }

}
