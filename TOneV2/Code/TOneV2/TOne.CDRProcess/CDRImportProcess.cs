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
    public partial class CDRImportProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            CDRImportProcessInput inputArg = createProcessInput.InputArguments as CDRImportProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("CDRImportProcessInput");
            return String.Format("CDR Import Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion
    }
}
