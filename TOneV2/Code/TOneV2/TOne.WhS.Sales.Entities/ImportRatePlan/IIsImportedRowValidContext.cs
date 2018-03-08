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

        string ErrorMessage { set; }
    }
}
