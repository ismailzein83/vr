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
    public partial class StoreInvalidCDRsInDBProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            StoreInvalidCDRsInDBProcessInput inputArg = createProcessInput.InputArguments as StoreInvalidCDRsInDBProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("StoreInvalidCDRsInDBProcessInput");
            return String.Format("Store Invalid CDRs for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}