using System;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseVRWorkflow
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public VRWorkflowSettings Settings { get; set; }
    }

    public class VRWorkflow : BaseVRWorkflow
    {
        public Guid VRWorkflowId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int LastModifiedBy { get; set; }
    }

    public class VRWorkflowSettings
    {
        public VRWorkflowArgumentCollection Arguments { get; set; }

        public VRWorkflowActivity RootActivity { get; set; }
    }

    public class VRWorkflowToAdd : BaseVRWorkflow
    {

    }

    public class VRWorkflowToUpdate : BaseVRWorkflow
    {
        public Guid VRWorkflowId { get; set; }
    }
}