using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public interface IDataTransformer : IBusinessManager
    {
        DataTransformationExecutionOutput ExecuteDataTransformation(Guid dataTransformationDefinitionId, Action<IDataTransformationExecutionContext> onContextReady);
    }
}
