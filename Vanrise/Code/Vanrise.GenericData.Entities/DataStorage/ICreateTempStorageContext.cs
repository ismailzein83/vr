using System;

namespace Vanrise.GenericData.Entities
{
    public interface ICreateTempStorageContext
    {
        DataStore DataStore { get; }

        DataRecordStorage DataRecordStorage { get; }

        long ProcessId { get; }

        TempStorageInformation TempStorageInformation { set; }
    }
}