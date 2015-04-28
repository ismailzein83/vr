using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TOne.CDRProcess
{
    public partial class BillingCDRsProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            BillingCDRsProcessInput inputArg = createProcessInput.InputArguments as BillingCDRsProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("BillingCDRsProcessInput");
            return String.Format("Billing CDRs Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
