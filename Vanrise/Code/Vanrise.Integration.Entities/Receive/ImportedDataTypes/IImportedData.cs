using System;

namespace Vanrise.Integration.Entities
{
    public interface IImportedData
    {
        string Description { get; }

        long? BatchSize { get; }

        BatchState BatchState { get; }

        bool? IsDuplicateSameSize { get; }

        bool IsEmpty { get; }

        bool IsFile { get; }

        bool IsMultipleReadings { get; }

        void OnDisposed();
    }
}