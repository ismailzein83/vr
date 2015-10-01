using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRulesByCriteria
    {
        public abstract bool IsEmpty();

        public abstract void SetSource(List<NormalizationRule> rules);

        public abstract NormalizationRule GetMostMatchedRule(int switchId, int trunkId, string phoneNumber);

        public NormalizationRulesByCriteria NextRuleSet { get; set; }
    }
}
