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
        public GetSummaryRecordStorageDataManagerContext(SummaryTransformationDefinition summaryTransformationDefinition, TempStorageInformation tempStorageInformation)
        {
            if (summaryTransformationDefinition == null)
                throw new ArgumentNullException("summaryTransformationDefinition");
            _summaryTransformationDefinition = summaryTransformationDefinition;

            _tempStorageInformation = tempStorageInformation;
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

        SummaryTransformationDefinition _summaryTransformationDefinition;
        public SummaryTransformationDefinition SummaryTransformationDefinition
        {
            get { return _summaryTransformationDefinition; }
        }

        TempStorageInformation _tempStorageInformation;
        public TempStorageInformation TempStorageInformation
        {
            get { return _tempStorageInformation; }
        }
    }
}
