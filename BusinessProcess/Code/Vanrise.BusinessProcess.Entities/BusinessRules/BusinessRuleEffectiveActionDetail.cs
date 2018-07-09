using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BusinessRuleEffectiveActionDetail
    {
        public BPBusinessRuleEffectiveAction Entity { get; set; }
        public Guid RuleDefinitionId { get; set; }
        public string ActionDescription { get; set; }
        public bool IsOverriden { get; set; }
        public string Description { get; set; }
        public List<Guid> ActionTypesIds { get; set; }
    }
}
