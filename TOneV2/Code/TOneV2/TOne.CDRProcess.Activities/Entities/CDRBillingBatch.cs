using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
  public  class CDRBillingBatch
    {
      public List<TABS.Billing_CDR_Base> CDRs { get; set; }

      public CDRBillingBatch()
      {
          CDRs = new List<TABS.Billing_CDR_Base>();
      }

    }
}
