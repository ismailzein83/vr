using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public interface IDataTransformationExecutionContext
    {
        int DataTransformationDefinitionId { get; }

        Dictionary<string, Object> DataRecords { get; }
    }
}
