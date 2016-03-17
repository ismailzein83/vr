using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRule
    {
        public int BPBusinessRuleId { get; set; }
        
        public BusinessRuleCondition Condition { get; set; }
        
        public string Description { get; set; }
        
        public string Key { get; set; }
        
        public List<int> ActionTypes { get; set; }

        public int DefaultActionId { get; set; }
    }
}
