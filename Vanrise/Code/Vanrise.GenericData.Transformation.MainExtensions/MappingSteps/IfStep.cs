using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class IfStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("1B22813C-F507-4BFB-BEE8-621F9F983ECB"); } }

        public string Condition { get; set; }

        public List<MappingStep> ThenSteps { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("if({0})", this.Condition);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.GenerateStepsCode(this.ThenSteps);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
