using System;

namespace Vanrise.Integration.Entities
{
    public class BaseAdapterArgument
    {
        public virtual int MaxParallelRuntimeInstances { get { return 1; } }

        public virtual bool IsFileDataSourceDefinitionInUse(Guid fileDataSourceDefinitionId)
        {
            return false;
        }
    }
}