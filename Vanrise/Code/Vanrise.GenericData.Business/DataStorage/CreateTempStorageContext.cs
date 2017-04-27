using System;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class CreateTempStorageContext : ICreateTempStorageContext
    {
        public DataStore DataStore { get; set; }

        public DataRecordStorage DataRecordStorage { get; set; }

        public long ProcessId { get; set; }

        public TempStorageInformation TempStorageInformation { get; set; }
    }
}