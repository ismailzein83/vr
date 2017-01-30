using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class TQISupplierInfoWithSummary
    {
        public IEnumerable<TQISuppplierInfo> SuppliersInfo { get; set; }

        public decimal? TotalDurationInMinutesSummary { get; set; }
    }
}
