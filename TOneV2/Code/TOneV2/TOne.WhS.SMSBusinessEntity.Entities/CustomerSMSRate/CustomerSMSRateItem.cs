using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateItem
    {
        public CustomerSMSRate CurrentRate { get; set; }

        public CustomerSMSRate FutureRate { get; set; }
    }
}
