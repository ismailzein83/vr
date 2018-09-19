using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime.Business;

namespace Vanrise.Integration.Business
{
    public class DataSourceManager : BaseBusinessEntityManager, IDataSourceManager
    {
        #region Public Methods

        public IEnumerable<DataSource> GetAllDataSources()
        {
            var cachedDataSources = GetCachedDataSources();
            if (cachedDataSources != null)
                return cachedDataSources.Values;
            else
                return null;
        }

        public IEnumerable<DataSource> GetEnabledDataSources()
        {
            var dataSources = GetAllDataSources();

            if (dataSources == null)
                return null;

            var enabledDataSources = new List<DataSource>();
            foreach (var dataSource in dataSources)
            {
                if (dataSource.IsEnabled)
                    enabledDataSources.Add(dataSource);
            }

            return enabledDataSources.Count > 0 ? enabledDataSources : null;
        }

        public List<Guid> GetEnabledDataSourcesIds()
        {
            var enabledDataSources = GetEnabledDataSources();
            if (enabledDataSources == null)
                return null;

            var enabledDataSourcesIds = new List<Guid>();
            foreach (var enabledDataSource in enabledDataSources)
            {
                enabledDataSourcesIds.Add(enabledDataSource.DataSourceId);
            }

            return enabledDataSourcesIds.Count > 0 ? enabledDataSourcesIds : null;
        }

        public Vanrise.Integration.Entities.DataSourceDetail GetDataSourceHistoryDetailbyHistoryId(int dataSourceHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var dataSource = s_vrObjectTrackingManager.GetObjectDetailById(dataSourceHistoryId);
            return dataSource.CastWithValidate<DataSourceDetail>("DataSourceDetail : historyId ", dataSourceHistoryId);
        }

        public IEnumerable<Vanrise.Integration.Entities.DataSourceInfo> GetDataSources(DataSourceFilter filter)
        {
            var dataSources = GetCachedDataSources();
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            if (filter != null)
            {
                return dataSources.MapRecords(DataSourceInfoMapper).Where(x => (!filter.AllExcept.Contains(x.DataSourceID)));
            }
            return dataSources.MapRecords(DataSourceInfoMapper);
        }

        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Integration.Entities.DataSourceDetail> GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<DataSourceQuery> input)
        {

            var cachedDataSources = GetCachedDataSources();
            Func<DataSource, bool> filterExpression = (dataSource) =>
                (input.Query.Name == null || dataSource.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                &&
                (input.Query.AdapterTypeIDs == null || input.Query.AdapterTypeIDs.Count() == 0 || input.Query.AdapterTypeIDs.Contains(dataSource.AdapterTypeId))
                &&
                (input.Query.IsEnabled == null || input.Query.IsEnabled == dataSource.IsEnabled);
            VRActionLogger.Current.LogGetFilteredAction(DataSourceLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataSources.ToBigResult(input, filterExpression, DataSourceDetailMapper));
        }

        public Vanrise.Integration.Entities.DataSourceDetail GetDataSourceDetail(Guid dataSourceId)
        {
            var cachedDataSources = GetCachedDataSources();
            var dataSource = cachedDataSources.GetRecord(dataSourceId);
            if (dataSource == null)
                return null;
            else return DataSourceDetailMapper(dataSource);
        }

        public DataSource GetDataSource(Guid dataSourceId)
        {
            return GetCachedDataSources().GetRecord(dataSourceId);
        }

        public string GetDataSourceName(Guid dataSourceId)
        {
            DataSource dataSource = GetDataSource(dataSourceId);

            if (dataSource != null)
                return dataSource.Name;

            return null;
        }

        public Vanrise.Integration.Entities.DataSource GetDataSourcebyTaskId(Guid taskId)
        {
            var cachedDataSources = GetCachedDataSources();
            return cachedDataSources.FindRecord(x => x.TaskId == taskId);
        }

        public IEnumerable<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<DataSourceAdapterType>(DataSourceAdapterType.EXTENSION_TYPE);
        }

        public Vanrise.Entities.InsertOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject,
            Vanrise.Runtime.Entities.SchedulerTask taskObject)
        {
            InsertOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> insertOperationOutput = new InsertOperationOutput<Vanrise.Integration.Entities.DataSourceDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            Vanrise.Runtime.Data.ISchedulerTaskDataManager taskDataManager = Vanrise.Runtime.Data.RuntimeDataManagerFactory.GetDataManager<Vanrise.Runtime.Data.ISchedulerTaskDataManager>();

            Vanrise.Runtime.Business.SchedulerTaskManager schedulerManager = new Runtime.Business.SchedulerTaskManager();
            taskObject.ActionTypeId = new Guid("B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68");
            taskObject.TriggerTypeId = new Guid("295B4FAC-DBF9-456F-855E-60D0B176F86B");
            Vanrise.Entities.InsertOperationOutput<Vanrise.Runtime.Entities.SchedulerTaskDetail> taskAdded = schedulerManager.AddTask(taskObject);
            dataSourceObject.DataSourceId = Guid.NewGuid();
            if (taskAdded.Result == InsertOperationResult.Succeeded)
            {
                dataSourceObject.TaskId = taskAdded.InsertedObject.Entity.TaskId;

                IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool dataSourceInsertActionSucc = dataManager.AddDataSource(dataSourceObject);

                if (dataSourceInsertActionSucc)
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    var dataSourceDetail = GetDataSourceDetail(dataSourceObject.DataSourceId);
                    VRActionLogger.Current.TrackAndLogObjectAdded(DataSourceLoggableEntity.Instance, dataSourceDetail);
                    insertOperationOutput.Result = InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = GetDataSourceDetail(dataSourceObject.DataSourceId);
                }
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject,
             Vanrise.Runtime.Entities.SchedulerTask taskObject)
        {
            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();

            bool dataSourceUpdateActionSucc = dataManager.UpdateDataSource(dataSourceObject);
            UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> updateOperationOutput = new UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            taskObject.ActionTypeId = new Guid("B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68");
            taskObject.TriggerTypeId = new Guid("295B4FAC-DBF9-456F-855E-60D0B176F86B");
            Vanrise.Runtime.Business.SchedulerTaskManager schedulerManager = new Runtime.Business.SchedulerTaskManager();
            Vanrise.Entities.UpdateOperationOutput<Vanrise.Runtime.Entities.SchedulerTaskDetail> taskUpdated = schedulerManager.UpdateTask(taskObject);

            if (taskUpdated.Result == UpdateOperationResult.Succeeded)
            {
                if (dataSourceUpdateActionSucc)
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    var dataSourceDetail = GetDataSourceDetail(dataSourceObject.DataSourceId);
                    VRActionLogger.Current.TrackAndLogObjectUpdated(DataSourceLoggableEntity.Instance, dataSourceDetail);
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = GetDataSourceDetail(dataSourceObject.DataSourceId);
                }
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteDataSource(Guid dataSourceId, Guid taskId)
        {
            var dataSource = GetDataSource(dataSourceId);
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            bool deleted = dataManager.DeleteDataSource(dataSourceId);

            if (deleted)
            {
                var dataSourceDetail = GetDataSourceDetail(dataSourceId);
                Vanrise.Runtime.Business.SchedulerTaskManager schedulerTaskManager = new Runtime.Business.SchedulerTaskManager();
                VRActionLogger.Current.TrackAndLogObjectDeleted(DataSourceLoggableEntity.Instance, dataSourceDetail);
                if ((schedulerTaskManager.DeleteTask(taskId)).Result == DeleteOperationResult.Succeeded)
                {

                    deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
                }
            }

            return deleteOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> AddDataSourceTask(Guid dataSourceId, Vanrise.Runtime.Entities.SchedulerTask task)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            Vanrise.Runtime.Business.SchedulerTaskManager schedulerManager = new Runtime.Business.SchedulerTaskManager();
            Vanrise.Entities.InsertOperationOutput<Vanrise.Runtime.Entities.SchedulerTaskDetail> taskAdded = schedulerManager.AddTask(task);

            if (taskAdded.Result == InsertOperationResult.Succeeded)
            {
                IDataSourceDataManager dataSourceDataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool updateActionSucc = dataSourceDataManager.UpdateTaskId(dataSourceId, taskAdded.InsertedObject.Entity.TaskId);

                if (updateActionSucc)
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                }
            }

            return updateOperationOutput;
        }

        public List<Vanrise.Queueing.Entities.QueueExecutionFlow> GetExecutionFlows()
        {
            QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
            return manager.GetExecutionFlows();
        }

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDetail> AddExecutionFlow(QueueExecutionFlow execFlowObject)
        {
            QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
            return manager.AddExecutionFlow(execFlowObject);
        }

        public List<QueueExecutionFlowDefinition> GetExecutionFlowDefinitions()
        {
            QueueExecutionFlowDefinitionManager manager = new QueueExecutionFlowDefinitionManager();
            return manager.GetAll();
        }

        public bool UpdateAdapterState(Guid dataSourceId, Vanrise.Integration.Entities.BaseAdapterState adapterState)
        {
            IDataSourceDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return manager.UpdateAdapterState(dataSourceId, adapterState);
        }

        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> DisableDataSource(Guid dataSourceId)
        {
            Vanrise.Runtime.Business.SchedulerTaskManager schedulerManager = new Runtime.Business.SchedulerTaskManager();

            UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> updateOperationOutput = new UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail>()
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            var dataSource = GetDataSource(dataSourceId);
            var taskUpdated = schedulerManager.DisableTask(dataSource.TaskId);

            if (taskUpdated.Result == UpdateOperationResult.Succeeded)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var dataSourceDetail = GetDataSourceDetail(dataSourceId);
                VRActionLogger.Current.LogObjectCustomAction(DataSourceLoggableEntity.Instance, "Disable", true, dataSourceDetail, null);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GetDataSourceDetail(dataSourceId);
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> EnableDataSource(Guid dataSourceId)
        {
            Vanrise.Runtime.Business.SchedulerTaskManager schedulerManager = new Runtime.Business.SchedulerTaskManager();

            UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> updateOperationOutput = new UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail>()
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            var dataSource = GetDataSource(dataSourceId);
            var dataSourceDetail = GetDataSourceDetail(dataSourceId);
            var taskUpdated = schedulerManager.EnableTask(dataSource.TaskId);

            if (taskUpdated.Result == UpdateOperationResult.Succeeded)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.LogObjectCustomAction(DataSourceLoggableEntity.Instance, "Enable", true, dataSourceDetail, null);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GetDataSourceDetail(dataSourceId);
            }

            return updateOperationOutput;
        }

        public bool DisableAllDataSource()
        {
            var cachedDataSources = GetCachedDataSources();
            foreach (KeyValuePair<Guid, DataSource> entry in cachedDataSources)
            {
                if (!entry.Value.IsEnabled)
                    continue;
                var output = DisableDataSource(entry.Value.DataSourceId);
                if (output.Result != UpdateOperationResult.Succeeded)
                    return false;
            }
            return true;
        }

        public bool EnableAllDataSource()
        {
            var cachedDataSources = GetCachedDataSources();
            foreach (KeyValuePair<Guid, DataSource> entry in cachedDataSources)
            {
                if (entry.Value.IsEnabled)
                    continue;
                var output = EnableDataSource(entry.Value.DataSourceId);
                if (output.Result != UpdateOperationResult.Succeeded)
                    return false;
            }
            return true;
        }

        public DataSourceManagmentInfo GetDataSourceManagmentInfo()
        {
            var dataSources = GetCachedDataSources();
            return new DataSourceManagmentInfo()
            {
                ShowEnableAll = dataSources.FindRecord(x => !x.IsEnabled) != null,
                ShowDisableAll = dataSources.FindRecord(x => x.IsEnabled) != null
            };
        }
        #endregion

        #region Private Methods

        private Dictionary<Guid, DataSource> GetCachedDataSources()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDataSources",
               () =>
               {
                   IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                   var dataSources = dataManager.GetAllDataSources();
                   if (dataSources != null)
                       return dataSources.ToDictionary(kvp => kvp.DataSourceId, kvp => kvp);
                   else
                       return null;
               });
        }

        #endregion

        #region Mappers

        DataSourceDetail DataSourceDetailMapper(DataSource dataSource)
        {
            var adapterTypes = GetDataSourceAdapterTypes();

            var adapterType = adapterTypes.FindRecord(x => x.ExtensionConfigurationId == dataSource.AdapterTypeId);

            return new DataSourceDetail()
            {
                AdapterInfo = adapterType,
                Entity = dataSource
            };
        }

        DataSourceInfo DataSourceInfoMapper(DataSource dataSource)
        {
            DataSourceInfo dataSourceInfo = new DataSourceInfo
            {
                DataSourceID = dataSource.DataSourceId,
                Name = dataSource.Name
            };
            return dataSourceInfo;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataSourceDataManager _dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            object _updateHandle;
            DateTime? _schedulerTaskCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataSourcesUpdated(ref _updateHandle)
                     |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SchedulerTaskManager.CacheManager>().IsCacheExpired(ref _schedulerTaskCacheLastCheck);
            }
        }

        private class DataSourceLoggableEntity : VRLoggableEntityBase
        {
            public static DataSourceLoggableEntity Instance = new DataSourceLoggableEntity();

            private DataSourceLoggableEntity()
            {

            }

            static DataSourceManager s_dataSourceManager = new DataSourceManager();

            public override string EntityUniqueName
            {
                get { return "VR_Integration_DataSource"; }
            }

            public override string ModuleName
            {
                get { return "Integration"; }
            }

            public override string EntityDisplayName
            {
                get { return "DataSource"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Integration_DataSource_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataSourceDetail dataSource = context.Object.CastWithValidate<DataSourceDetail>("context.Object");
                return dataSource.Entity.DataSourceId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataSourceDetail dataSource = context.Object.CastWithValidate<DataSourceDetail>("context.Object");
                return s_dataSourceManager.GetDataSourceName(dataSource.Entity.DataSourceId);
            }
        }

        #endregion

        #region IBusinessEntityManager

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetDataSource(context.EntityId);
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var dataSources = GetCachedDataSources();
            if (dataSources == null)
                return null;
            else
                return dataSources.Values.Select(itm => itm as dynamic).ToList();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetDataSourceName(new Guid(context.EntityId.ToString()));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var dataSource = context.Entity as DataSource;
            return dataSource.DataSourceId;
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
