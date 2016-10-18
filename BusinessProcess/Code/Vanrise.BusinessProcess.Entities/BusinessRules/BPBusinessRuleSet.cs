using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleSet
    {
        public int BPBusinessRuleSetId { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public Guid BPDefinitionId { get; set; }

        public BPBusinessRuleSetDetails Details { get; set; }
    }

    public class BPBusinessRuleSetDetails
    {
        public List<BPBusinessRuleActionDetails> ActionDetails { get; set; }
    }
}
