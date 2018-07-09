using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleEffectiveAction
    {
        public Guid RuleDefinitionId { get; set; }

        public BusinessRuleAction Action { get; set; }

        public bool IsInherited { get; set; }
    }
}
