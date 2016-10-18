using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public abstract class MappingStep
    {
        public abstract Guid ConfigId { get; }
        public abstract void GenerateExecutionCode(IDataTransformationCodeGenerationContext context);
    }
}
