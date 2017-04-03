using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IDataRecordTypeDataManager : IDataManager
    {
        List<DataRecordType> GetDataRecordTypes();

        bool AreDataRecordTypeUpdated(ref object updateHandle);

        bool UpdateDataRecordType(DataRecordType dataRecordType);

        bool AddDataRecordType(DataRecordType dataRecordType);

        void SetDataRecordTypeCacheExpired();
    }
}