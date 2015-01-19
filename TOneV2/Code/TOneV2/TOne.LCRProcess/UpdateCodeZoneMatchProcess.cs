using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using TOne.LCRProcess.Arguments;

namespace TOne.LCRProcess
{
    public partial class UpdateCodeZoneMatchProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            UpdateCodeZoneMatchProcessInput inputArg = createProcessInput.InputArguments as UpdateCodeZoneMatchProcessInput;
            if (inputArg == null)
                throw new ArgumentNullException("UpdateCodeZoneMatchProcessInput");
            return String.Format("Update CodeMatch & ZoneMatch Process at {0: dd-MMM-yyyy HH:mm:ss}", DateTime.Now);
        }

        #endregion
    }
}
