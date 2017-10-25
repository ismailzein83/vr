using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRateQuery
    {
        public int SupplierId { get; set; }
        public bool ShowPending { get; set; }
        public List<int> CountriesIds { get; set; }
        public string SupplierZoneName { get; set; }
    }
}
