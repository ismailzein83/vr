using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface IIsImportedRowValidContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        ImportedRow ImportedRow { get; }

        ZoneChanges ZoneDraft { get; }

        SaleZone ExistingZone { get; }

        Dictionary<int, DateTime> CountryBEDsByCountry { get; }

        IEnumerable<int> ClosedCountryIds { get; }

        ImportedRowStatus Status { set; }

        string DateTimeFormat { get; }

        bool AllowRateZero { get; }

        string ErrorMessage { set; }

        Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }
        int LongPrecision { get; set; }
    }

    public interface IIsImportedCustomerTargetMatchRowValidContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        CustomerTargetMatchImportedRow ImportedRow { get; }

        ZoneChanges ZoneDraft { get; }

        SaleZone ExistingZone { get; }

        Dictionary<int, DateTime> CountryBEDsByCountry { get; }

        IEnumerable<int> ClosedCountryIds { get; }

        CustomerTargetMatchImportedRowStatus Status { set; }

        string DateTimeFormat { get; }

        bool AllowRateZero { get; }

        string ErrorMessage { set; }

        //Dictionary<int, DateTime> AdditionalCountryBEDsByCountryId { get; set; }

        RateCalculationMethod RateCalculationMethod { get; set; }
        
        int LongPrecision { get; set; }

        Func<long, ZoneItem> GetZoneItem { get; set; }

        Func<Guid, int?> GetCostCalculationMethodIndex { get; set; }
    }
}
