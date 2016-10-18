using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class IfElseStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("9cf3c165-1921-4f83-990d-03b82a04aa5a"); } }

        public string Condition { get; set; }

        public List<MappingStep> ThenSteps { get; set; }

        public List<MappingStep> ElseSteps { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("if({0})", this.Condition);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.GenerateStepsCode(this.ThenSteps);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
            context.AddCodeToCurrentInstanceExecutionBlock("else");
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.GenerateStepsCode(this.ElseSteps);
            context.AddCodeToCurrentInstanceExecutionBlock("}");     
        }
    }
}
