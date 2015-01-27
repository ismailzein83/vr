using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    public class CDRMain
    {
        List<TABS.Billing_CDR_Main> mainCDRs { get; set; }

        public CDRMain()
        {
            mainCDRs = new List<TABS.Billing_CDR_Main>();
        }
    }
}
