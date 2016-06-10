using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.Caching;
namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldChoiceManager
    {     
   
        #region Public Methods

        public IEnumerable<DataRecordFieldChoiceInfo> GetDataRecordFieldChoicesInfo()
        {
            return this.GetCachedDataRecordFieldChoices().MapRecords(DataRecordFieldChoiceInfoMapper).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataRecordFieldChoiceDetail> GetFilteredDataRecordFieldChoices(Vanrise.Entities.DataRetrievalInput<DataRecordFieldChoiceQuery> input)
        {
            var cachedDataRecordFieldChoice = GetCachedDataRecordFieldChoices();
            Func<DataRecordFieldChoice, bool> filterExpression = (dataRecordFieldChoice) => (input.Query.Name == null || dataRecordFieldChoice.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataRecordFieldChoice.ToBigResult(input, filterExpression, DataRecordFieldChoiceDetailMapper));
        }

        public DataRecordFieldChoice GeDataRecordFieldChoice(int dataRecordFieldChoiceId)
        {
            var cachedDataRecordFieldChoice = GetCachedDataRecordFieldChoices();
            return cachedDataRecordFieldChoice.FindRecord((dataRecordFieldChoice) => dataRecordFieldChoice.DataRecordFieldChoiceId == dataRecordFieldChoiceId);
        }

        public Vanrise.Entities.InsertOperationOutput<DataRecordFieldChoiceDetail> AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            InsertOperationOutput<DataRecordFieldChoiceDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordFieldChoiceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataRecordFieldChoiceId = -1;

            IDataRecordFieldChoiceDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
            bool insertActionSucc = dataManager.AddDataRecordFieldChoice(dataRecordFieldChoice, out dataRecordFieldChoiceId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataRecordFieldChoice.DataRecordFieldChoiceId = dataRecordFieldChoiceId;
                insertOperationOutput.InsertedObject = DataRecordFieldChoiceDetailMapper(dataRecordFieldChoice);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataRecordFieldChoiceDetail> UpdateDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            UpdateOperationOutput<DataRecordFieldChoiceDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordFieldChoiceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDataRecordFieldChoiceDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
            bool updateActionSucc = dataManager.UpdateDataRecordFieldChoice(dataRecordFieldChoice);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = DataRecordFieldChoiceDetailMapper(dataRecordFieldChoice);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        Dictionary<int, DataRecordFieldChoice> GetCachedDataRecordFieldChoices()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordFieldChoices",
                () =>
                {
                    IDataRecordFieldChoiceDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
                    IEnumerable<DataRecordFieldChoice> dataRecordFieldChoices = dataManager.GetDataRecordFieldChoices();
                    return dataRecordFieldChoices.ToDictionary(dataRecordFieldChoice => dataRecordFieldChoice.DataRecordFieldChoiceId, dataRecordFieldChoice => dataRecordFieldChoice);
                });
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordFieldChoiceDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordFieldChoicesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        DataRecordFieldChoiceInfo DataRecordFieldChoiceInfoMapper(DataRecordFieldChoice dataRecordFieldChoice)
        {
            return new DataRecordFieldChoiceInfo() {
                 DataRecordFieldChoiceId = dataRecordFieldChoice.DataRecordFieldChoiceId,
                 Name = dataRecordFieldChoice.Name
            };
        }

        DataRecordFieldChoiceDetail DataRecordFieldChoiceDetailMapper(DataRecordFieldChoice dataRecordFieldChoice)
        {
            return new DataRecordFieldChoiceDetail()
            {
                Entity = dataRecordFieldChoice
            };
        }

        #endregion
    }
}
