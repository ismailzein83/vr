﻿using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowInputArgument : BaseProcessInputArgument
    {
        public override string ProcessName { get { return string.Format("VRWorkflowInputArgument_{0}", BPDefinitionId.ToString("N")); } }

        public Guid BPDefinitionId { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        public override string GetTitle()
        {
            var bpDefinition = BusinessManagerFactory.GetManager<IBPDefinitionManager>().GetBPDefinition(this.BPDefinitionId);
            var vrWorkflow = BusinessManagerFactory.GetManager<IVRWorkflowManager>().GetVRWorkflow(bpDefinition.VRWorkflowId.Value);
            return vrWorkflow.Title;
        }
    }
}