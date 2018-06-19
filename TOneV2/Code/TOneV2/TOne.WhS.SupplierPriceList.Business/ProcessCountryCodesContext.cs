using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ProcessCountryCodesContext : IProcessCountryCodesContext
    {
        public int CountryId { get; set; }

        public SupplierPricelistType SupplierPriceListType { get; set; }

        public IEnumerable<ImportedZone> ImportedZones { get; set; }

        public IEnumerable<ImportedCode> ImportedCodes { get; set; }

        public IEnumerable<ExistingCode> ExistingCodes { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public DateTime DeletedCodesDate { get; set; }
        public DateTime PriceListDate { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<NewCode> NewCodes { get; set; }

        public IEnumerable<ChangedCode> ChangedCodes { get; set; }

        public IEnumerable<NewZone> NewZones { get; set; }

        public IEnumerable<ChangedZone> ChangedZones { get; set; }

        public IEnumerable<NotImportedCode> NotImportedCodes { get; set; }
    }
}
