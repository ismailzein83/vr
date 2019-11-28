﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.Caching;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldChoiceManager
    {
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #region Public Methods

        public IEnumerable<DataRecordFieldChoiceInfo> GetDataRecordFieldChoicesInfo()
        {
            Func<DataRecordFieldChoice, bool> filterExpression = (dataRecordFieldChoice) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(dataRecordFieldChoice.DevProjectId))
                    return false;

                return true;
            };
            return this.GetCachedDataRecordFieldChoices().MapRecords(DataRecordFieldChoiceInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataRecordFieldChoiceDetail> GetFilteredDataRecordFieldChoices(Vanrise.Entities.DataRetrievalInput<DataRecordFieldChoiceQuery> input)
        {
            var cachedDataRecordFieldChoice = GetCachedDataRecordFieldChoices();
            Func<DataRecordFieldChoice, bool> filterExpression = (dataRecordFieldChoice) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(dataRecordFieldChoice.DevProjectId))
                    return false;

                if (input.Query.Name != null && !dataRecordFieldChoice.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                    return false;
                if (input.Query.DevProjectIds != null && (!dataRecordFieldChoice.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(dataRecordFieldChoice.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(DataRecordFieldChoiceLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataRecordFieldChoice.ToBigResult(input, filterExpression, DataRecordFieldChoiceDetailMapper));

        }

        public DataRecordFieldChoice GetDataRecordFieldChoice(Guid dataRecordFieldChoiceId)
        {
            throw new NotImplementedException();
        }

        public DataRecordFieldChoice GeDataRecordFieldChoice(Guid dataRecordFieldChoiceId)
        {
            var cachedDataRecordFieldChoice = GetCachedDataRecordFieldChoices();
            return cachedDataRecordFieldChoice.FindRecord((dataRecordFieldChoice) => dataRecordFieldChoice.DataRecordFieldChoiceId == dataRecordFieldChoiceId);
        }

        public string GetDataRecordFieldChoiceName(DataRecordFieldChoice dataRecordFieldChoice)
        {
            if (dataRecordFieldChoice != null)
                return dataRecordFieldChoice.Name;

            return null;
        }

        public Vanrise.Entities.InsertOperationOutput<DataRecordFieldChoiceDetail> AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            InsertOperationOutput<DataRecordFieldChoiceDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordFieldChoiceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            dataRecordFieldChoice.DataRecordFieldChoiceId = Guid.NewGuid();
            IDataRecordFieldChoiceDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
            bool insertActionSucc = dataManager.AddDataRecordFieldChoice(dataRecordFieldChoice);

            if (insertActionSucc)
            {
                VRActionLogger.Current.TrackAndLogObjectAdded(DataRecordFieldChoiceLoggableEntity.Instance, dataRecordFieldChoice);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(DataRecordFieldChoiceLoggableEntity.Instance, dataRecordFieldChoice);
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

        public Dictionary<Guid, DataRecordFieldChoice> GetCachedDataRecordFieldChoices()
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

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordFieldChoiceDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordFieldChoicesUpdated(ref _updateHandle);
            }
        }

        private class DataRecordFieldChoiceLoggableEntity : VRLoggableEntityBase
        {
            public static DataRecordFieldChoiceLoggableEntity Instance = new DataRecordFieldChoiceLoggableEntity();

            private DataRecordFieldChoiceLoggableEntity()
            {

            }

            static DataRecordFieldChoiceManager s_dataRecordFieldChoiceManager = new DataRecordFieldChoiceManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_DataRecordFieldChoice"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Data Record Field Choice"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_DataRecordFieldChoice_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataRecordFieldChoice dataRecordFieldChoice = context.Object.CastWithValidate<DataRecordFieldChoice>("context.Object");
                return dataRecordFieldChoice.DataRecordFieldChoiceId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataRecordFieldChoice dataRecordFieldChoice = context.Object.CastWithValidate<DataRecordFieldChoice>("context.Object");
                return s_dataRecordFieldChoiceManager.GetDataRecordFieldChoiceName(dataRecordFieldChoice);
            }
        }

        #endregion

        #region Mappers

        DataRecordFieldChoiceInfo DataRecordFieldChoiceInfoMapper(DataRecordFieldChoice dataRecordFieldChoice)
        {
            return new DataRecordFieldChoiceInfo() {
                 DataRecordFieldChoiceId = dataRecordFieldChoice.DataRecordFieldChoiceId,
                 Name = vrDevProjectManager.ConcatenateTitleAndDevProjectName(dataRecordFieldChoice.DevProjectId, dataRecordFieldChoice.Name)
            };
        }

        DataRecordFieldChoiceDetail DataRecordFieldChoiceDetailMapper(DataRecordFieldChoice dataRecordFieldChoice)
        {
            string devProjectName = null;
            if (dataRecordFieldChoice.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(dataRecordFieldChoice.DevProjectId.Value);
            }
            return new DataRecordFieldChoiceDetail()
            {
                Entity = dataRecordFieldChoice,
                DevProjectName= devProjectName
            };
        }

        #endregion
    }
}
