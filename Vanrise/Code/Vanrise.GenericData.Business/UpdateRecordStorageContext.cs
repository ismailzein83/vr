using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class UpdateRecordStorageContext : IUpdateRecordStorageContext
    {
        DataStore _dataStore;
        DataRecordStorage _dataRecordStorage;
        Object _recordStorageState;

        public UpdateRecordStorageContext(DataStore dataStore, DataRecordStorage dataRecordStorage, Object recordStorageState)
        {
            _dataRecordStorage = dataRecordStorage;
            _dataStore = dataStore;
            _recordStorageState = recordStorageState;
        }

        public DataStore DataStore
        {
            get { return _dataStore; }
        }
        public DataRecordStorage RecordStorage
        {
            get { return _dataRecordStorage; }
        }
        public Object RecordStorageState
        {
            get { return _recordStorageState; }
            set { _recordStorageState = value; }
        }
    }
}
