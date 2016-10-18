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
        public override Guid ConfigId { get { return new Guid("9c158fa5-8516-4af7-aedd-1bc69d026afc"); } }

        public string ArrayVariableName { get; set; }
        public string VariableName { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Add({1});", this.ArrayVariableName, VariableName);      
        }
    }
}
