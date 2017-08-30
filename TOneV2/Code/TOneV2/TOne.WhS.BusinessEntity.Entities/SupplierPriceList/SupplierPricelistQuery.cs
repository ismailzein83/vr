using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierPricelistQuery
    {
        public List<int?> SupplierIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<int> UserIds { get; set; }
    }
}
