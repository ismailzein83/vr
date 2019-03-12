using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees
{
    public class VRWorkflowSpecificUsersTaskAssignees : VRWorkflowTaskAssigneesSetting
    {
        public override Guid ConfigId => new Guid("B55A355A-66BA-4036-AC47-261B80C93CA2");

        public string UserIds { get; set; }

        public override string GetAssigneesCode()
        {
            return string.Concat("return new Vanrise.BusinessProcess.MainExtensions.FixedUsersBPTaskAssignee { UserIds = ", this.UserIds, "};");
        }
    }
}
