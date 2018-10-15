using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class ProcessedCountryDataInfo
    {
        public int CountryId { get; set; }
        public IEnumerable<NewCode> NewCodes { get; set; }
        public IEnumerable<NewZone> NewZones { get; set; }
        public IEnumerable<NewRate> NewRates { get; set; }
        public IEnumerable<NewZoneService> NewZonesServices { get; set; }
        public IEnumerable<ChangedCode> ChangedCodes { get; set; }
        public IEnumerable<ChangedZone> ChangedZones { get; set; }
        public IEnumerable<ChangedZoneService> ChangedZoneServices { get; set; }
        public IEnumerable<ChangedRate> ChangedRates { get; set; }
        public IEnumerable<ImportedCode> ImportedCodes { get; set; }
        public IEnumerable<ImportedZone> ImportedZones { get; set; }
        public IEnumerable<ImportedRate> ImportedRates { get; set; }
        public List<NotImportedZone> NotImportedZones { get; set; }
        public List<NotImportedCode> NotImportedCodes { get; set; }
    }

}
