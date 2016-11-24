using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class StopIterationStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("CB929C72-D08A-410A-B9E1-2B5D29D417FA"); } }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("continue;");
        }
    }
}
