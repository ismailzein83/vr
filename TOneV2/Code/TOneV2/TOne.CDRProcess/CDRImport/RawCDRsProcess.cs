using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess
{
    public partial class RawCDRsProcess : Activity, IBPWorkflow
    {
        public string GetTitle(Vanrise.BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            RawCDRsProcessInput inputArg = createProcessInput.InputArguments as RawCDRsProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("RawCDRsProcessInput");
            return String.Format("Raw CDRs Process for Switch {0}", inputArg.SwitchID);
        }
    }
}
