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
    public partial class TimeRangeRepricingProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            TimeRangeRepricingProcessInput inputArg = createProcessInput.InputArguments as TimeRangeRepricingProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("DailyRepricingProcessInput");
            return String.Format("Time Range Repricing Process for range {0:HH:mm} - {1:HH:mm} in date {2:dd-MMM-yyyy}", inputArg.Range.From, inputArg.Range.To, inputArg.Range.From);
        }

        #endregion
    }
}
