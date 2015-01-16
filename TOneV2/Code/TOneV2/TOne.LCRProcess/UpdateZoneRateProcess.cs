using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess
{
    public partial class UpdateZoneRateProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            return String.Format("Update ZoneRate Process at {0: dd-MMM-yyyy HH:mm:ss}", DateTime.Now);
        }

        #endregion
    }
}
