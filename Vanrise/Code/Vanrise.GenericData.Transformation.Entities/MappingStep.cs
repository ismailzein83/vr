using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public abstract class MappingStep
    {
        public abstract void Execute(IMappingStepExecutionContext context);

        public virtual void GenerateExecutionCode(IDataTransformationCodeContext context)
        {

        }
    }
}
