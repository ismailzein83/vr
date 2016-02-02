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
            DataTransformationDefinitionManager definitionManager = new DataTransformationDefinitionManager();
            DataTransformationRuntimeType dataTransformationRuntimeType = definitionManager.GetTransformationRuntimeType(context.DataTransformationDefinitionId);
            var executor = Activator.CreateInstance(dataTransformationRuntimeType.ExecutorType) as IDataTransformationExecutor;
            if(context.DataRecords != null)
            {
                foreach(var dataRecordEntry in context.DataRecords)
                {
                    executor.AddDataRecord(dataRecordEntry.Key, dataRecordEntry.Value);
                }
            }
            if(dataTransformationRuntimeType.DataRecordTypes != null)
            {
                foreach(var dataRecordTypeEntry in dataTransformationRuntimeType.DataRecordTypes)
                {
                    if(context.DataRecords == null || !context.DataRecords.ContainsKey(dataRecordTypeEntry.Key))
                    {
                        executor.AddDataRecord(dataRecordTypeEntry.Key, Activator.CreateInstance(dataRecordTypeEntry.Value));
                    }
                }
            }
            executor.Execute();
        }
    }
}
