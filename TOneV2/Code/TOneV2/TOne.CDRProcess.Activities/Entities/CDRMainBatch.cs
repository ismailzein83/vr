using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    public class CDRMainBatch
    {
       public List<TABS.Billing_CDR_Main> mainCDRs { get; set; }

        public CDRMainBatch()
        {
            mainCDRs = new List<TABS.Billing_CDR_Main>();
        }
    }
}
