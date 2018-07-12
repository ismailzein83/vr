using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleEffectiveActionQuery
    {
        public int? BusinessRuleSetDefinitionId { get; set; }
        public Guid BusinessProcessId { get; set; }
        public int? ParentBusinessRuleSetId { get; set; }
    }
}
