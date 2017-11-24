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
        public override Guid ConfigId { get { return new Guid("60435713-b35e-4e38-a538-9a479061472e"); } }

        public Guid DataTransformationId { get; set; }
        public List<RecordMapping> RecordsMapping { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            context.AddCodeToCurrentInstanceExecutionBlock("var transformationDefinitionManager = new Vanrise.GenericData.Transformation.DataTransformationDefinitionManager();");
            context.AddCodeToCurrentInstanceExecutionBlock("var dataTransformationRuntimeType = transformationDefinitionManager.GetTransformationRuntimeType(new Guid(\"{0}\"));", this.DataTransformationId);
            context.AddCodeToCurrentInstanceExecutionBlock("dynamic transformationExecutor = Activator.CreateInstance(dataTransformationRuntimeType.ExecutorType);");

            foreach (var recordMapping in this.RecordsMapping)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("transformationExecutor.{0} = {1};", recordMapping.RecordName, recordMapping.Value);
            }

            context.AddCodeToCurrentInstanceExecutionBlock("((Vanrise.GenericData.Transformation.IDataTransformationExecutor)transformationExecutor).Execute();");

            foreach (var recordMapping in this.RecordsMapping)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = transformationExecutor.{1};", recordMapping.Value, recordMapping.RecordName);
            } 

            //context.AddCodeToCurrentInstanceExecutionBlock("var dataTransformer = new Vanrise.GenericData.Transformation.DataTransformer();");
            //context.AddCodeToCurrentInstanceExecutionBlock("var executionOutput =dataTransformer.ExecuteDataTransformation(new Guid(\"{0}\"),(context)=>",this.DataTransformationId);
            //context.AddCodeToCurrentInstanceExecutionBlock("{");

            //foreach (var recordMapping in this.RecordsMapping)
            //{
            //    context.AddCodeToCurrentInstanceExecutionBlock(" context.SetRecordValue(\"{0}\", {1});", recordMapping.RecordName, recordMapping.Value);
            //}

            //context.AddCodeToCurrentInstanceExecutionBlock("});");

            //foreach (var recordMapping in this.RecordsMapping)
            //{
            //    context.AddCodeToCurrentInstanceExecutionBlock("{0} = executionOutput.GetRecordValue(\"{1}\");", recordMapping.Value, recordMapping.RecordName);
            //}     
        }
    }
}
