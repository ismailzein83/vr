using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BaseProcessSynchronisation
    {
        public string Name { get; set; }
        public ProcessSynchronisationSettings Settings { get; set; }
    }

    public class ProcessSynchronisationToAdd : BaseProcessSynchronisation
    {

    }
    public class ProcessSynchronisationToUpdate : BaseProcessSynchronisation
    {
        public Guid ProcessSynchronisationId { get; set; }
    }

    public class ProcessSynchronisation : BaseProcessSynchronisation
    {
        public Guid ProcessSynchronisationId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }

    public class ProcessSynchronisationSettings
    {
        public ProcessSynchronisationGroup FirstProcessSynchronisationGroup { get; set; }
        public ProcessSynchronisationGroup SecondProcessSynchronisationGroup { get; set; }
    }

    public class ProcessSynchronisationGroup
    {
        public List<BPSynchronisationItem> BPSynchronisationItems { get; set; }
        public List<ExecutionFlowSynchronisationItem> ExecutionFlowSynchronisationItems { get; set; }
    }

    public class BPSynchronisationItem
    {
        public Guid BPDefinitionId { get; set; }
        public List<Guid> TaskIds { get; set; }
    }

    public class ExecutionFlowSynchronisationItem
    {
        public Guid ExecutionFlowDefinitionId { get; set; }
    }
}