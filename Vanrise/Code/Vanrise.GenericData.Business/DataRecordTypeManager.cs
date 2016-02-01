using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.Business
{
    public class DataRecordTypeManager
    {
       
        #region Public Methods
        public IDataRetrievalResult<DataRecordTypeDetail> GetFilteredDataRecordTypes(DataRetrievalInput<DataRecordTypeQuery> input)
        {
            var allItems = GetCachedDataRecordTypes();

            Func<DataRecordType, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, DataRecordTypeDetailMapper));
        }
        public DataRecordType GetDataRecordType(int dataRecordTypeId)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            return dataRecordTypes.GetRecord(dataRecordTypeId);
        }
        public string GetDataRecordTypeName(int? dataRecordTypeId)
        {
            if (dataRecordTypeId == null)
                return null;
            var dataRecordTypes = GetCachedDataRecordTypes();

            DataRecordType dataRecordType = dataRecordTypes.GetRecord((int)dataRecordTypeId);

            if (dataRecordType != null)
              return dataRecordType.Name;

            return null;
        }
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypeInfo(DataRecordTypeInfoFilter filter)
        {
            var dataRecordTypes = GetCachedDataRecordTypes();
            return dataRecordTypes.MapRecords(DataRecordTypeInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail> AddDataRecordType(DataRecordType dataRecordType)
        {
            InsertOperationOutput<DataRecordTypeDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataRecordTypeId = -1;


            IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            bool insertActionSucc = dataManager.AddDataRecordType(dataRecordType, out dataRecordTypeId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataRecordTypes");
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataRecordType.DataRecordTypeId = dataRecordTypeId;
                insertOperationOutput.InsertedObject = DataRecordTypeDetailMapper(dataRecordType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<DataRecordTypeDetail> UpdateDataRecordType(DataRecordType dataRecordType)
        {
            IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            bool updateActionSucc = dataManager.UpdateDataRecordType(dataRecordType);
            UpdateOperationOutput<DataRecordTypeDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataRecordTypes");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataRecordTypeDetailMapper(dataRecordType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public List<Vanrise.Entities.TemplateConfig> GetDataRecordFieldTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.DataRecordFieldConfigType);
        }
        #endregion
      
        #region Private Methods
        private Dictionary<int, DataRecordType> GetCachedDataRecordTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordTypes",
               () =>
               {
                   IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                   IEnumerable<DataRecordType> dataRecordTypes = dataManager.GetDataRecordTypes();
                   return dataRecordTypes.ToDictionary(kvp => kvp.DataRecordTypeId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordTypeDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordTypeUpdated(ref _updateHandle);
            }
        }


        #endregion

        #region Mappers

        private DataRecordTypeDetail DataRecordTypeDetailMapper(DataRecordType dataRecordTypeObject)
        {
            DataRecordTypeDetail dataRecordTypeDetail = new DataRecordTypeDetail();
            dataRecordTypeDetail.Entity = dataRecordTypeObject;
            dataRecordTypeDetail.ParentName = GetDataRecordTypeName(dataRecordTypeObject.ParentId);
            return dataRecordTypeDetail;
        }
        private DataRecordTypeInfo DataRecordTypeInfoMapper(DataRecordType dataRecordTypeObject)
        {
            DataRecordTypeInfo dataRecordTypeInfo = new DataRecordTypeInfo();
            dataRecordTypeInfo.DataRecordTypeId = dataRecordTypeObject.DataRecordTypeId;
            dataRecordTypeInfo.Name =  dataRecordTypeObject.Name;
            return dataRecordTypeInfo;
        }
        #endregion
    }
}
