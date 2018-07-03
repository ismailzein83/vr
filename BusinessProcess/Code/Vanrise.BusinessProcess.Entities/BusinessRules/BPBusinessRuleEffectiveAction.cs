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

        public Guid ActionId { get; set; }

        public bool IsOverriden { get; set; }
    }
}
