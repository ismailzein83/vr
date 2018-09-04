using System;

namespace Vanrise.Reprocess.Entities{
    public interface IReprocessDefinitionManager : IReprocessManager
    {
        ReprocessDefinition GetReprocessDefinition(Guid reprocessDefinitionId); 
    }
}
