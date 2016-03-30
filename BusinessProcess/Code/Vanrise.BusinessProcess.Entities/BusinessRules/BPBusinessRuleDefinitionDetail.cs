using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleDefinitionDetail
    {
        public BPBusinessRuleDefinition Entity { get; set; }

        public List<BPBusinessRuleActionType> ActionTypes { get; set; }
    }
}
