using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string ErrorMessage { get; set; }
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

        public string ErrorMessage { get; set; }
    }
}
