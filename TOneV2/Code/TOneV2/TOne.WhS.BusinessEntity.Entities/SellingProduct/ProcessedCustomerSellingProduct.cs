using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ProcessedCustomerSellingProduct : Vanrise.Entities.IDateEffectiveSettingsEditable
    {
        public int CustomerId { get; set; }

        public int SellingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
