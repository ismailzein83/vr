using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class TelephonyLineSubscriptionInput
    {
        public string PhoneNumber { get; set; }
        public string RatePlanId { get; set; }
        public List<TelephoneService> Services { get; set; }

    }

    public class TelephonyLineSubscriptionOutput
    {

    }
}
