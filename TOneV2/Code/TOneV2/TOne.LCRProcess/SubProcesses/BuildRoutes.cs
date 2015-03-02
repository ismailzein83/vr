using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess
{
    public partial class BuildRoutes : Activity, IBPWorkflow
    {
        
        public string GetTitle(Vanrise.BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            return "Build Routes Process";
        }
    }
}
