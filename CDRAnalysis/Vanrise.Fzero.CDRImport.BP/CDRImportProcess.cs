using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Fzero.CDRImport.BP
{
    public partial class CDRImportProcess : Activity, IBPWorkflow
    {
        public string GetTitle(CreateProcessInput createProcessInput)
        {
            return "CDRImportProcess";
        }
    }
}
