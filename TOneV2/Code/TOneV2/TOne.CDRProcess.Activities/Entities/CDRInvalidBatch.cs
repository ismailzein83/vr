using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    public class CDRInvalidBatch
    {
      public  List<TABS.Billing_CDR_Invalid> InvalidCDRs { get; set; }

        public CDRInvalidBatch()
        {
            InvalidCDRs = new List<TABS.Billing_CDR_Invalid>();
        }
    }
}
