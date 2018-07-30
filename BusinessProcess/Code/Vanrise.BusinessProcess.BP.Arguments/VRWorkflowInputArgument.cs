using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.BP.Arguments
{
    public class VRWorkflowInputArgument : BaseProcessInputArgument
    {
        public override string ProcessName { get { return string.Format("VRWorkflowInputArgument_{0}", BPDefinitionId.ToString("N")); } }

        public Guid BPDefinitionId { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        public override string GetTitle()
        {
            var bpDefinition = new BPDefinitionManager().GetBPDefinition(this.BPDefinitionId);
            var vrWorkflow = new VRWorkflowManager().GetVRWorkflow(bpDefinition.VRWorkflowId.Value);
            return vrWorkflow.Title;
        }
    }
}