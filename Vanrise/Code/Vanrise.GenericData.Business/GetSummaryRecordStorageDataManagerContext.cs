using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class GetSummaryRecordStorageDataManagerContext : IGetSummaryRecordStorageDataManagerContext
    {
        SummaryTransformationDefinition _summaryTransformationDefinition;
        public GetSummaryRecordStorageDataManagerContext(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            if (summaryTransformationDefinition == null)
                throw new ArgumentNullException("summaryTransformationDefinition");
            _summaryTransformationDefinition = summaryTransformationDefinition;
        }

        DataStore _dataStore;
        public DataStore DataStore
        {
            get
            {
                if (_dataStore == null)
                {
                    DataStoreManager manager = new DataStoreManager();
                    _dataStore = manager.GetDataStore(this.DataRecordStorage.DataStoreId);
                    if (_dataStore == null)
                        throw new NullReferenceException("_dataStore");
                }
                return _dataStore;
            }
        }

        DataRecordStorage _dataRecordStorage;
        public DataRecordStorage DataRecordStorage
        {
            get
            {
                if (_dataRecordStorage == null)
                {
                    DataRecordStorageManager manager = new DataRecordStorageManager();
                    _dataRecordStorage = manager.GetDataRecordStorage(this.SummaryTransformationDefinition.DataRecordStorageId);
                    if (_dataRecordStorage == null)
                        throw new NullReferenceException("_dataRecordStorage");
                }
                return _dataRecordStorage;
            }
        }

        public SummaryTransformationDefinition SummaryTransformationDefinition
        {
            get { return _summaryTransformationDefinition; }
        }
    }
}
