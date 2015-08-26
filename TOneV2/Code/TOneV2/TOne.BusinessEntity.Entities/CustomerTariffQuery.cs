using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CustomerTariffQuery
    {
        public string selectedCustomerID { get; set; }
        public List<int> selectedZoneIDs { get; set; }
        public DateTime effectiveOn { get; set; }
    }
}
