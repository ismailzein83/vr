using System;

namespace Vanrise.Integration.Entities
{
    public interface IImportedData
    {
        string Description { get; }

        long? BatchSize { get; }

        bool IsEmpty { get; }

        bool IsFile { get; }

        bool IsMultipleReadings { get; }

        void OnDisposed();
    }
}