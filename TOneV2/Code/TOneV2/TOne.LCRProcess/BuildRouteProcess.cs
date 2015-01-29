using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TOne.LCRProcess
{
    public partial class BuildRouteProcess : Activity, IBPWorkflow
    {
        #region IBPWorkflow Members

        public string GetTitle(CreateProcessInput createProcessInput)
        {
            return String.Format("Update ZoneRate Process at {0: dd-MMM-yyyy HH:mm:ss}", DateTime.Now);
        }

        #endregion
    }
}

