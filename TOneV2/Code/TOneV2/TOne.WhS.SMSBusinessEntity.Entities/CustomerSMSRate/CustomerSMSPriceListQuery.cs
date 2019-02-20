using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSPriceListQuery
    {
        public List<int> CustomerIds { get; set; }

        public DateTime? EffectiveDate { get; set; }
    }
}
