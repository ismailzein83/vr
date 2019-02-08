using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSPriceList
    {
        public long ID { get; set; }
        public int CustomerID { get; set; }

        public string CurrencyID { get; set; }

        public DateTime EffectiveOn { get; set; }

        public long? ProcessInstanceID { get; set; }
        
        public int UserID { get; set; }
    }
}
