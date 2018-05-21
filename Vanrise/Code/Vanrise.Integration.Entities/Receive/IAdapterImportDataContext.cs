using System;

namespace Vanrise.Integration.Entities
{
    public interface IAdapterImportDataContext
    {
        Guid DataSourceId { get; }

        BaseAdapterArgument AdapterArgument { get; }
        
        void GetStateWithLock(Func<BaseAdapterState, BaseAdapterState> onStateReady);

        ImportedBatchProcessingOutput OnDataReceived(IImportedData data);

        void StartNewInstanceIfAllowed();

        bool ShouldStopImport();
    }
}
