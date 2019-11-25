using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRateQuery
    {
        public int SupplierId { get; set; }
        public bool ShowPending { get; set; }
        public List<int> CountriesIds { get; set; }
        public string SupplierZoneName { get; set; }
        public bool ByCode { get; set; }
        public List<string> ColumnsToShow { get; set; }
    }
}
