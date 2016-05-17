using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.MainExtensions.MappingSteps
{
    public class BELookupRuleMappingStep : Vanrise.GenericData.Transformation.Entities.MappingStep
    {
        public int BELookupRuleDefinitionId { get; set; }

        public string EffectiveTime { get; set; }

        public List<BELookupRuleCriteriaFieldMapping> CriteriaFieldsMappings { get; set; }

        public string BusinessEntity { get; set; }

        public override void GenerateExecutionCode(Transformation.Entities.IDataTransformationCodeGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class BELookupRuleCriteriaFieldMapping
    {
        public string FieldPath { get; set; }

        public string Value { get; set; }
    }
}
