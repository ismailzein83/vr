using System;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleAction
    {
        public int BPBusinessRuleActionId { get; set; }

        public BPBusinessRuleActionDetails Details { get; set; }
    }

    public class BPBusinessRuleActionDetails
    {
        public Guid BPBusinessRuleDefinitionId { get; set; }

        public BPBusinessRuleActionSettings Settings { get; set; }
    }

    public class BPBusinessRuleActionSettings
    {
        public BusinessRuleAction Action { get; set; }
        public bool Disabled { get; set; }
    }
}
