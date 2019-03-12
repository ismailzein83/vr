using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees
{
    public class VRWorkflowProcessInitiatorTaskAssignees : VRWorkflowTaskAssigneesSetting
    {
        public override Guid ConfigId => new Guid("D297A8B8-997D-4127-91FA-353238296940");

        public override string GetAssigneesCode()
        {
            return "return new Vanrise.BusinessProcess.MainExtensions.InitiatorBPTaskAssignee();";
        }
    }
}
