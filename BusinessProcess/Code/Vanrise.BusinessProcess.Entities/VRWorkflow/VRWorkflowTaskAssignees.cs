using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowTaskAssignees
    {
        public VRWorkflowTaskAssigneesSetting Settings { get; set; }
    }

    public abstract class VRWorkflowTaskAssigneesSetting
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetAssigneesCode();
    }
}
