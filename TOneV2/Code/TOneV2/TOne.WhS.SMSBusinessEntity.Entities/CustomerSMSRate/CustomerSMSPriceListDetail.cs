using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSPriceListDetail
    {
        public long ID { get; set; }

        public string CustomerName { get; set; }

        public string CurrencyName { get; set; }

        public DateTime EffectiveOn { get; set; }

        public string UserName { get; set; }
    }
}