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
        public string Source { get; set; }

        public string Target { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1};", 
                this.Target, this.Source);
        }
    }
}
