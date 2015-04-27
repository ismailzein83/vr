using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;

namespace Vanrise.Fzero.FraudAnalysis.BP
{
    public partial class TestProcess : Activity, IBPWorkflow
    {
        public string GetTitle(BusinessProcess.Entities.CreateProcessInput createProcessInput)
        {
            return "TestProcess";
        }
    }
}
