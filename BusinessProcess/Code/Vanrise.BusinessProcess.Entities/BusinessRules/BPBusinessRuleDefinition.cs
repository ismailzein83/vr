using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleDefinition
    {
        public int BPBusinessRuleDefinitionId { get; set; }

        public int BPDefintionId { get; set; }

        public string Name { get; set; }

        public BPBusinessRuleSettings Settings { get; set; }
    }

    public class BPBusinessRuleSettings
    {
        public BusinessRuleCondition Condition { get; set; }
        public List<int> ActionTypes { get; set; }
        public string Description { get; set; }
    }
}
