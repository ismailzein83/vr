using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees
{
    class VRWorkflowSpecificGroupsTaskAssignees : VRWorkflowTaskAssigneesSetting
    {
        public override Guid ConfigId => new Guid("F9C477A8-4374-4C58-A859-02F5A26EDCD7");

        public string GroupIds { get; set; }

        public override string GetAssigneesCode()
        {
            return string.Concat("return new Vanrise.BusinessProcess.MainExtensions.GroupsBPTaskAssignee { GroupIds = ", this.GroupIds, "};");
        }
    }
}
