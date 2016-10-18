using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class AssignFieldStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("00e8e50c-017e-44ed-96a9-6d4291a9c4b6"); } }

        public string Source { get; set; }

        public string Target { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1};", 
                this.Target, this.Source);
        }
    }
}
