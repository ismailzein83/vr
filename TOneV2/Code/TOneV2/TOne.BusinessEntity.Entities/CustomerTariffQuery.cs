using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CustomerTariffQuery
    {
        public string SelectedCustomerID { get; set; }
        public List<int> SelectedZoneIDs { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
