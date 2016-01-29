using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    public class DataTransformer
    {
        public void ExecuteDataTransformation(IDataTransformationExecutionContext context)
        {
            DataTransformationDefinition dataTransformationDefinition = GetDataTransformationDefinition(context.DataTransformationDefinitionId);
            var stepExecutionContext = new MappingStepExecutionContext
            {
                DataRecords = context.DataRecords
            };
            if (stepExecutionContext.DataRecords == null)
                stepExecutionContext.DataRecords = new Dictionary<string, DataRecord>();
            foreach(var dataRecordType in dataTransformationDefinition.RecordTypes)
            {
                if (!stepExecutionContext.DataRecords.ContainsKey(dataRecordType.Key))
                    stepExecutionContext.DataRecords.Add(dataRecordType.Key, new DataRecord() { FieldsValues = new Dictionary<string, object>() });
            }
            foreach (var step in dataTransformationDefinition.MappingSteps)
            {
                step.Execute(stepExecutionContext);
            }
        }

        private DataTransformationDefinition GetDataTransformationDefinition(int dataTransformationDefinitionId)
        {
            throw new NotImplementedException();
        }
    }
}
