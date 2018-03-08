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
    }
}
