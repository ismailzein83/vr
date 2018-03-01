using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using System.Activities;

namespace Vanrise.BusinessProcess
{
    internal class BPRunningInstance
    {
        public BPInstance BPInstance { get; set; }
        public WorkflowApplication WFApplication { get; set; }

        public bool IsIdle { get; set; }

        public bool IsWorkflowCompleted { get; set; }
    }
}
