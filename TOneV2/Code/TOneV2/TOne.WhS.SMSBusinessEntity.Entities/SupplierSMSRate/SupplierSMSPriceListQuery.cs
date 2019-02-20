using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSPriceListQuery
    {
        public List<int> SupplierIds { get; set; }

        public DateTime? EffectiveDate { get; set; }
    }
}
