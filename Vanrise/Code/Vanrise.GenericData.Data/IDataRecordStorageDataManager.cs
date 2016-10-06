using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IDataRecordStorageDataManager : IDataManager
    {
        IEnumerable<DataRecordStorage> GetDataRecordStorages();

        bool AddDataRecordStorage(DataRecordStorage dataRecordStorage);

        bool UpdateDataRecordStorage(DataRecordStorage dataRecordStorage);

        bool AreDataRecordStoragesUpdated(ref object updateHandle);
    }
}
