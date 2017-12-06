
namespace Vanrise.Integration.Entities
{
    public interface IImportedData
    {
        string Description { get; }

        long? BatchSize { get; }

        void OnDisposed();

        bool IsMultipleReadings { get; }

        bool IsEmpty { get; }
        bool IsFile { get; }
    }
}
