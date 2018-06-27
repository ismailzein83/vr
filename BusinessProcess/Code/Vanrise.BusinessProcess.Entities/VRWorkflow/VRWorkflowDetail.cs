using System;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowDetail
    {
        public Guid VRWorkflowID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}