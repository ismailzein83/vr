using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflow
    {
        public Guid VRWorkflowId { get; set; }

        public string Name { get; set; }

        public VRWorkflowSettings Settings { get; set; }
    }

    public class VRWorkflowSettings
    {
        public VRWorkflowArgumentCollection Arguments { get; set; }

        public VRWorkflowActivity RootActivity { get; set; }
    }
}
