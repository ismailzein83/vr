using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class SubscriberItem
    {
        public long SubscriberId { get; set; }

        public SubscriberType Type { get; set; }
    }
}
