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
        public void ExecuteDataTransformation(int dataTransformationDefinitionId, Action<IDataTransformationExecutionContext> onContextReady)
        {
            DataTransformationDefinitionManager definitionManager = new DataTransformationDefinitionManager();
            DataTransformationRuntimeType dataTransformationRuntimeType = definitionManager.GetTransformationRuntimeType(dataTransformationDefinitionId);
            var executionContext = new DataTransformationExecutionContext(dataTransformationRuntimeType.ExecutorType);
            if (onContextReady != null)
                onContextReady(executionContext);
            executionContext.Executor.Execute();
        }
    }

    internal class DataTransformationExecutionContext : IDataTransformationExecutionContext
    {
        Type _executorType;
        IDataTransformationExecutor _executor;

        internal IDataTransformationExecutor Executor
        {
            get
            {
                return this._executor;
            }
        }

        public DataTransformationExecutionContext(Type executorType)
        {
            if (executorType == null)
                throw new ArgumentNullException("executorType");
            this._executorType = executorType;
            this._executor = Activator.CreateInstance(executorType) as IDataTransformationExecutor;
            if (this._executor == null)
                throw new Exception(String.Format("{0} is not of type IDataTransformationExecutor", executorType));
        }

        public void SetRecordValue(string recordName, object recordValue)
        {
            this._executorType.GetField(recordName).SetValue(this._executor, recordValue);
        }
    }
}
