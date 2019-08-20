using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class VRWorkflowInputArgument : BaseProcessInputArgument
    {
        public abstract Dictionary<string, object> ConvertArgumentsToDictionary();
    }

    /// <summary>
    /// Temporary
    /// </summary>
    public class VRWorkflowDictInputArgument : VRWorkflowInputArgument
    {
        public Guid BPDefinitionId { get; set; }

        public Dictionary<string, Object> InputArguments { get; set; }

        public override string ProcessName { get { return string.Format("VRWorkflowInputArgument_{0} ", this.BPDefinitionId.ToString("N")); } }

        public override Dictionary<string, object> ConvertArgumentsToDictionary()
        {
            return this.InputArguments;
        }

        public override string GetTitle()
        {
            return "New Instance";
        }
    }

}