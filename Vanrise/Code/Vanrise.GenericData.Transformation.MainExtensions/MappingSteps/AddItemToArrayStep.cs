using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class AddItemToArrayStep : MappingStep
    {
        public string ArrayVariableName { get; set; }
        public string VariableName { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var arrayVariableName = context.GenerateUniqueMemberName(this.ArrayVariableName.ToLower());
            context.AddCodeToCurrentInstanceExecutionBlock("if({0} == null)",arrayVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = new List<{1}>();",arrayVariableName, this.ArrayVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Add({1}",arrayVariableName,this.VariableName);      
        }
    }
}
