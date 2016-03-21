namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleAction
    {
        public int BPBusinessRuleActionId { get; set; }

        public int BPBusinessRuleDefinitionId { get; set; }

        public BPBusinessRuleActionSettings Settings { get; set; }
    }

    public class BPBusinessRuleActionSettings
    {
        public BusinessRuleAction Action { get; set; }
    }
}
