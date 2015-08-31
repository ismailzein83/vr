using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class SupplierTariffQuery
    {
        public string SelectedSupplierID { get; set; }
        public List<int> SelectedZoneIDs { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
