using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using TOne.CDRProcess.Arguments;

namespace TOne.CDRProcess
{
    public partial class UpdateBillingPricingProcess : Activity, IBPWorkflow
    {
        public string GetTitle(Vanrise.BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            UpdateBillingPricingProcessInput inputArg = createProcessInput.InputArguments as UpdateBillingPricingProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("UpdateBillingPricingProcess");
            return String.Format("Update Billing And Pricing Process");
        }
    }
}
