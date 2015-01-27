using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    public class CDRInvalid
    {
        List<TABS.Billing_CDR_Invalid> InvalidCDRs { get; set; }

        public CDRInvalid()
        {
            InvalidCDRs = new List<TABS.Billing_CDR_Invalid>();
        }
    }
}
