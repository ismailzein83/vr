using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCRProcess.Arguments;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess
{
    public partial class RoutingByCodeGroupProcess : Activity, IBPWorkflow
    {
        public string GetTitle(Vanrise.BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            RoutingByCodeGroupProcessInput inputArg = createProcessInput.InputArguments as RoutingByCodeGroupProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("RoutingByCodeGroupProcessInput");
            return String.Format("Routing Process for Codes starts by {0}", inputArg.CodePrefix);
        }
    }
}
