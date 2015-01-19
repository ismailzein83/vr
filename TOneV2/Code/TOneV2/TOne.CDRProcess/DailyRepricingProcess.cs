using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess;
using System.Activities;
using Vanrise.BusinessProcess.Entities;
using TOne.CDRProcess.Arguments;

namespace TOne.CDRProcess
{
    public partial class DailyRepricingProcess : Activity, IBPWorkflow
    {

        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            DailyRepricingProcessInput inputArg = createProcessInput.InputArguments as DailyRepricingProcessInput;
            if(inputArg == null)
                throw new ArgumentNullException("DailyRepricingProcessInput");
            return String.Format("Daily Repricing Process for date {0:dd-MMM-yyyy}", inputArg.RepricingDay);
        }

        #endregion
    }
}
