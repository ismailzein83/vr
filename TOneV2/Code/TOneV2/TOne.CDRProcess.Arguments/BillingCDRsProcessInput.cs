using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class BillingCDRsProcessInput
    {
        public int SwitchID { get; set; }

        public Guid CacheManagerId { get; set; }
    }
}
