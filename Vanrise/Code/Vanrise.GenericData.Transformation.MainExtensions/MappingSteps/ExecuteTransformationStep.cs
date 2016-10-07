using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ExecuteTransformationStep : MappingStep
    {
        public int DataTransformationId { get; set; }
        public List<RecordMapping> RecordsMapping { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("var dataTransformer = new Vanrise.GenericData.Transformation.DataTransformer();");
            context.AddCodeToCurrentInstanceExecutionBlock("var executionOutput =dataTransformer.ExecuteDataTransformation(new Guid(\"{0}\"),(context)=>",this.DataTransformationId);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            foreach (var recordMapping in this.RecordsMapping)
            {
                context.AddCodeToCurrentInstanceExecutionBlock(" context.SetRecordValue(\"{0}\", {1});", recordMapping.RecordName, recordMapping.Value);
            }

            context.AddCodeToCurrentInstanceExecutionBlock("});");

            foreach (var recordMapping in this.RecordsMapping)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = executionOutput.GetRecordValue(\"{1}\");", recordMapping.Value, recordMapping.RecordName);
            }     
        }
    }
}
