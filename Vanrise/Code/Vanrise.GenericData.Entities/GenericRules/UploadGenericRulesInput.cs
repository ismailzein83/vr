using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class UploadGenericRulesInput
    {
        public Guid GenericRuleDefinitionId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public long FileId { get; set; }

        public Dictionary<string, CriteriaFieldsValues> CriteriaFieldsValues { get; set; }
    }
}
