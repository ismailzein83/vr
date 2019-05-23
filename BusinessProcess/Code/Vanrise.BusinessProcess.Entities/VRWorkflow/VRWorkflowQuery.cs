
using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowQuery
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public List<Guid> DevProjectIds { get; set; }
    }
}
