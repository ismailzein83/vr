using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class StructuredProcessSynchronisation
    {
        public LinkedProcessSynchronisationItems LinkedProcessSynchronisationItems { get; private set; }
        public Dictionary<Guid, LinkedProcessSynchronisationItems> LinkedProcessSynchronisationItemsByTaskId { get; private set; }

        public StructuredProcessSynchronisation()
        {
            this.LinkedProcessSynchronisationItems = new LinkedProcessSynchronisationItems();
            this.LinkedProcessSynchronisationItemsByTaskId = new Dictionary<Guid, LinkedProcessSynchronisationItems>();
        }
    }

    public class LinkedProcessSynchronisationItems
    {
        public LinkedProcessSynchronisationItems()
        {
            this.TaskIds = new HashSet<Guid>();
            this.BPDefinitionIds = new HashSet<Guid>();
            this.ExecutionFlowDefinitionIds = new HashSet<Guid>();
        }
        public HashSet<Guid> TaskIds { get; private set; }

        public HashSet<Guid> BPDefinitionIds { get; private set; }

        public HashSet<Guid> ExecutionFlowDefinitionIds { get; private set; }
    }
}