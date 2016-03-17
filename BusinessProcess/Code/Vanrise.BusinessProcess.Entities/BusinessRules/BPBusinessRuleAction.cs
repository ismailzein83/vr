namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleAction
    {
        public int BusinessRuleActionId { get; set; }
               
        public string Description { get; set; }

        public BusinessRuleAction Action { get; set; }
    }
}
