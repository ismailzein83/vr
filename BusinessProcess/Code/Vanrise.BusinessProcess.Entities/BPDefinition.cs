using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinition
    {
        public int BPDefinitionID { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public Type WorkflowType { get; set; } // Type should inherit System.Activities.Activity
        public BPConfiguration Configuration { get; set; }
    }

    public class BPConfiguration
    {
        public int? MaxConcurrentWorkflows { get; set; }
        public string Url { get; set; }
        public string ScheduleTemplateURL { get; set; }
        public string ManualExecEditor { get; set; }
        public string ScheduledExecEditor { get; set; }
        public bool IsPersistable { get; set; }
        public bool HasChildProcesses { get; set; }
        public bool HasBusinessRules { get; set; }
        public bool NotVisibleInManagementScreen { get; set; }
        public BPDefinitionSecurity Security { get; set; }
    }

    public class BPDefinitionSecurity
    {
        public RequiredPermissionSettings View { get; set; }
        public RequiredPermissionSettings StartNewInstance { get; set; }
        public RequiredPermissionSettings ScheduleTask { get; set; }

    }
}