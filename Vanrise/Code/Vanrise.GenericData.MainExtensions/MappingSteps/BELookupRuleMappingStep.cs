using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.MappingSteps
{
    public class BELookupRuleMappingStep : Vanrise.GenericData.Transformation.Entities.MappingStep
    {

        public override Guid ConfigId { get { return new Guid("43932599-404b-4f18-9f5b-963915bf16dc"); } }


        public Guid BELookupRuleDefinitionId { get; set; }

        public string EffectiveTime { get; set; }

        public List<BELookupRuleCriteriaFieldMapping> CriteriaFieldsMappings { get; set; }

        public string BusinessEntity { get; set; }

        public override void GenerateExecutionCode(Transformation.Entities.IDataTransformationCodeGenerationContext context)
        {
            var ruleTargetVariableName = context.GenerateUniqueMemberName("ruleTarget");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new {1}();", ruleTargetVariableName, CSharpCompiler.TypeToString(typeof(GenericRuleTarget)));
            if (!String.IsNullOrEmpty(this.EffectiveTime))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.EffectiveOn = {1};", ruleTargetVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetFieldValues = new Dictionary<string, object>();", ruleTargetVariableName);

            foreach (var ruleFieldMapping in this.CriteriaFieldsMappings)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null) {1}.TargetFieldValues.Add(\"{2}\", {0});",
                    ruleFieldMapping.Value, ruleTargetVariableName, ruleFieldMapping.FieldPath);
            }

            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Business.BELookupRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.GetMatchBE(new Guid(\"{2}\"), {3});", this.BusinessEntity, ruleManagerVariableName, this.BELookupRuleDefinitionId, ruleTargetVariableName);
        }
    }

    public class BELookupRuleCriteriaFieldMapping
    {
        public string FieldPath { get; set; }

        public string Value { get; set; }
    }
}
