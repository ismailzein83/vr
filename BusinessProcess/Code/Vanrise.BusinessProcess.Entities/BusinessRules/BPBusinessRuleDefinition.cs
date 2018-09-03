using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleDefinition
    {
        public Guid BPBusinessRuleDefinitionId { get; set; }

        public Guid BPDefintionId { get; set; }

        public string Name { get; set; }

        public BPBusinessRuleSettings Settings { get; set; }

        public int Rank { get; set; }
    }

    public class BPBusinessRuleSettings
    {
        public BusinessRuleCondition Condition { get; set; }
        
        public List<Guid> ActionTypes { get; set; }

        public List<Guid> ExecutionDependsOnRules { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public bool CanBeDisabled { get; set; }
    }
}
