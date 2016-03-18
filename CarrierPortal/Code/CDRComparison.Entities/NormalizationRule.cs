using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class NormalizationRule
    {
        public string FieldToNormalize { get; set; }

        public List<RuleCriteriaField> CriteriaFields { get; set; }

        public Vanrise.Rules.Normalization.NormalizeNumberSettings NormalizationSettings { get; set; }
    }
}
