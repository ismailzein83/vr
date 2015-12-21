using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Business.GenericDataRecord
{
    public class DataRecordTypeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<DataRecordTypeDetail> GetFilteredDataRecordTypes(Vanrise.Entities.DataRetrievalInput<DataRecordTypeQuery> input)
        {
            var dataRecordTypes = GetChachedDataRecordTypes();

            Func<DataRecordType, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower())) ;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataRecordTypes.ToBigResult(input, filterExpression, MapToDetails));
        }
        public DataRecordType GetDataRecordType(int? dataRecordTypeId)
        {
            if (dataRecordTypeId == null)
                return null;

            var dataRecordTypes = GetChachedDataRecordTypes();
            var returnedObj=dataRecordTypes.FindRecord(x => x.DataRecordTypeId == dataRecordTypeId);
            if(returnedObj.Fields==null)
                returnedObj.Fields=new List<DataRecordField>();
            return returnedObj;
        }

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordTypeDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordTypeUpdated(ref _updateHandle);
            }
        }
        protected Dictionary<int, DataRecordType> GetChachedDataRecordTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetChachedDataRecordTypes",
              () =>
              {
                  IDataRecordTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                  var dataRecordTypes= dataManager.GetALllDataRecordTypes();
                  return dataRecordTypes.ToDictionary(x => x.DataRecordTypeId, x => x);
              });
        }
        private DataRecordTypeDetail MapToDetails(DataRecordType dataRecordType)
        {  
            DataRecordType parentTypeName=GetDataRecordType(dataRecordType.ParentId);
            return new DataRecordTypeDetail
            {
                Entity = dataRecordType,
                ParentName = parentTypeName!=null?parentTypeName.Name:null
            };
        }

        #endregion
    }
}
