using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Entities.DataStorage.DataRecordStorage;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataRecordStorageFilter : IDataRecordStorageFilter
    {
        public bool IsMatched(DataRecordStorage dataRecordStorage)
        {
            if (dataRecordStorage == null)
                throw new NullReferenceException("dataRecordStorage");

            if (dataRecordStorage.Settings == null)
                throw new NullReferenceException("dataRecordStorage.Settings");

            if (dataRecordStorage.Settings is RDBDataRecordStorageSettings)
                return true;

            return false;
        }
    }
}
