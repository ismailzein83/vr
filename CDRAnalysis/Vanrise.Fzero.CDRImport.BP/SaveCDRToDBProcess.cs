using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;

namespace Vanrise.Fzero.CDRImport.BP
{
    public partial class SaveCDRToDBProcess : Activity, IBPWorkflow
    {

        public string GetTitle(BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            return "Save CDR To Database";
        }
    }
}
