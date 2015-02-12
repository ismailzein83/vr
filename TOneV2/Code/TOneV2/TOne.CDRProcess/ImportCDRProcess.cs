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
    public partial class ImportCDRProcess : Activity, IBPWorkflow
    {

        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            ImportCDRProcessInput inputArg = createProcessInput.InputArguments as ImportCDRProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("ImportCDRProcessInput");
            return String.Format("Import CDR Process for Switch {0}", inputArg.SwitchID);
        }

        #endregion

    }
}
