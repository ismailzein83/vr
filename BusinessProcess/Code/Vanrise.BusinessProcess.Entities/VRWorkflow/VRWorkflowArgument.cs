using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public enum VRWorkflowArgumentDirection { In = 0, Out = 1, InOut = 2}
    public class VRWorkflowArgument
    {
        public Guid VRWorkflowArgumentId { get; set; }

        public string Name { get; set; }

        public VRWorkflowVariableType Type { get; set; }

        public VRWorkflowArgumentDirection Direction { get; set; }
    }

    public class VRWorkflowArgumentCollection : List<VRWorkflowArgument>
    {

    }
}
