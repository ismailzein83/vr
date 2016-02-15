using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ForLoopStep : MappingStep
    {
        public List<MappingStep> Steps { get; set; }

        public string ArrayVariableName { get; set; }

        public string IterationVariableName { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("foreach(var {0} in {1})", this.IterationVariableName, this.ArrayVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.GenerateStepsCode(this.Steps);
            context.AddCodeToCurrentInstanceExecutionBlock("}");     
        }
    }
}
