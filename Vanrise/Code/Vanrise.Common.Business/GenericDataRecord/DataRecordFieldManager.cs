using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Business.GenericDataRecord
{
    public class DataRecordFieldManager
    {
        readonly DataRecordTypeManager _dataRecordTypeManager;
        public DataRecordFieldManager()
        {
            _dataRecordTypeManager = new DataRecordTypeManager();

        }
        public Vanrise.Entities.IDataRetrievalResult<DataRecordFieldDetail> GetFilteredDataRecordFields(Vanrise.Entities.DataRetrievalInput<DataRecordFieldQuery> input)
        {

            var dataRecordType = GetChachedDataRecordFields();

            Func<DataRecordField, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                &&
                (input.Query.TypeIds == null || input.Query.TypeIds.Contains(prod.Type.ConfigId))
                &&
                (input.Query.DataRecordTypeId == prod.DataRecordTypeID);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataRecordType.ToBigResult(input, filterExpression, MapToDetails));
        }
        public Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail> UpdateDataRecordField(DataRecordField dataRecordField)
        {
            Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordFieldDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IDataRecordFieldDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordFieldDataManager>();

            var dataRecordFields = GetChachedDataRecordFields();
            bool updateActionSucc = false;
            if (dataRecordFields.Any(x => x.Key == dataRecordField.ID))
            {
                var records = dataRecordFields.FindAllRecords(x => x.DataRecordTypeID == dataRecordField.DataRecordTypeID);
                if (records.Any(x => x.Name == dataRecordField.Name && x.ID != dataRecordField.ID))
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
                else
                {
                    updateActionSucc = _dataManager.Update(dataRecordField);
                }

            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(dataRecordField);
            }

            return updateOperationOutput;

        }
        public Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail> DeleteDataRecordField(int dataRecordFieldId)
        {
            Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<DataRecordFieldDetail>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            IDataRecordFieldDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordFieldDataManager>();
            var dataRecordFields = GetChachedDataRecordFields();
            bool deleteActionSucc = false;
            if (dataRecordFields.Any(x => x.Key == dataRecordFieldId))
            {
                deleteActionSucc = _dataManager.Delete(dataRecordFieldId);
            }
            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail> AddDataRecordField(DataRecordField dataRecordField)
        {

            Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordFieldDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int insertedId = -1;
            IDataRecordFieldDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordFieldDataManager>();
            bool insertActionSucc = false;
            var dataRecordFields = GetChachedDataRecordFields();
            if (!dataRecordFields.Any(x => x.Value.Name == dataRecordField.Name && x.Value.DataRecordTypeID == dataRecordField.DataRecordTypeID))
            {
                insertActionSucc = _dataManager.Insert(dataRecordField, out  insertedId);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataRecordField.ID = insertedId;
                insertOperationOutput.InsertedObject = MapToDetails(dataRecordField);
            }

            return insertOperationOutput;
        }
        public DataRecordField GetDataRecordField(int dataRecordFieldId)
        {
            var dataRecordFields = GetChachedDataRecordFields();
            return dataRecordFields.FindRecord(x => x.ID == dataRecordFieldId);
        }
        public List<Vanrise.Entities.TemplateConfig> GetDataRecordFieldTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CDRFieldConfigType);
        }

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordFieldDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordFieldDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordFieldUpdated(ref _updateHandle);
            }
        }
        protected Dictionary<int, DataRecordField> GetChachedDataRecordFields()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetChachedDataRecordFields",
              () =>
              {
                  IDataRecordFieldDataManager dataManager = CommonDataManagerFactory.GetDataManager<IDataRecordFieldDataManager>();
                  var dataRecordTypes = dataManager.GetALllDataRecordFields();

                  return dataRecordTypes.ToDictionary(x => x.ID, x => x);
              });
        }
        private DataRecordFieldDetail MapToDetails(DataRecordField dataRecordField)
        {
            DataRecordType parentTypeName = _dataRecordTypeManager.GetDataRecordType(dataRecordField.DataRecordTypeID);
            var templates = GetDataRecordFieldTypeTemplates();
            var template = templates.FindRecord(x => x.TemplateConfigID == dataRecordField.Type.ConfigId);

            return new DataRecordFieldDetail
            {
                Entity = dataRecordField,
                DataRecordTypeDescription = parentTypeName != null ? parentTypeName.Name : null,
                TypeDescription = template != null ? template.Name : null
            };
        }

        #endregion
    }
}
