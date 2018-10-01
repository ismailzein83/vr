using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class StructuredProcessSynchronisation
    {
        public LinkedProcessSynchronisationItems LinkedProcessSynchronisationItems { get; set; }

        public Dictionary<Guid, LinkedProcessSynchronisationItems> LinkedProcessSynchronisationItemsByTaskId { get; set; }
    }

    public class LinkedProcessSynchronisationItems
    {
        public HashSet<Guid> TaskIds { get; set; }

        public HashSet<Guid> BPDefinitionIds { get; set; }

        public HashSet<Guid> ExecutionFlowDefinitionIds { get; set; }
    }
}