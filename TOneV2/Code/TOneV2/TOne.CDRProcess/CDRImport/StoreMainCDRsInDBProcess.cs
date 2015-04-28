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
    public partial class StoreMainCDRsInDBProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            StoreMainCDRsInDBProcessInput inputArg = createProcessInput.InputArguments as StoreMainCDRsInDBProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("StoreMainCDRsInDBProcessInput");
            return String.Format("Store Main CDRs for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
