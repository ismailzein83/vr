using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SupplierTariffQuery
    {
        public string selectedSupplierID { get; set; }
        public List<int> selectedZoneIDs { get; set; }
        public DateTime effectiveOn { get; set; }
    }
}
