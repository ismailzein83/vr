using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Integration.Business
{
    public class DataSourceManager : IDataSourceManager, IBusinessEntityManager
    {
        public IEnumerable<DataSource> GetAllDataSources()
        {
            var cachedDataSources = GetCachedDataSources();
            if (cachedDataSources != null)
                return cachedDataSources.Values;
            else
                return null;
        }
        public IEnumerable<Vanrise.Integration.Entities.DataSourceInfo> GetDataSources(DataSourceFilter filter)
        {
            var dataSources = GetCachedDataSources();
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            if (filter !=null)
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
            return cachedDataSources.FindRecord(x=>x.TaskId == taskId);
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
            Vanrise.Entities.InsertOperationOutput<Vanrise.Runtime.Entities.SchedulerTask> taskAdded = schedulerManager.AddTask(taskObject);
            dataSourceObject.DataSourceId = Guid.NewGuid();
            if (taskAdded.Result == InsertOperationResult.Succeeded)
            {
                dataSourceObject.TaskId = taskAdded.InsertedObject.TaskId;
                
                IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool dataSourceInsertActionSucc = dataManager.AddDataSource(dataSourceObject);

                if (dataSourceInsertActionSucc)
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject =  GetDataSourceDetail(dataSourceObject.DataSourceId);
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
            Vanrise.Entities.UpdateOperationOutput<Vanrise.Runtime.Entities.SchedulerTask> taskUpdated = schedulerManager.UpdateTask(taskObject);

            if (taskUpdated.Result == UpdateOperationResult.Succeeded)
            {
                if (dataSourceUpdateActionSucc)
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = GetDataSourceDetail(dataSourceObject.DataSourceId);
                }
            }
            
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteDataSource(Guid dataSourceId, Guid taskId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            bool deleted = dataManager.DeleteDataSource(dataSourceId);

            if (deleted)
            {
                Vanrise.Runtime.Business.SchedulerTaskManager schedulerTaskManager = new Runtime.Business.SchedulerTaskManager();
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
            Vanrise.Entities.InsertOperationOutput<Vanrise.Runtime.Entities.SchedulerTask> taskAdded = schedulerManager.AddTask(task);

            if (taskAdded.Result==InsertOperationResult.Succeeded)
            {
                IDataSourceDataManager dataSourceDataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool updateActionSucc = dataSourceDataManager.UpdateTaskId(dataSourceId, taskAdded.InsertedObject.TaskId);
                
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

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetDataSource(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var dataSources = GetCachedDataSources();
            if (dataSources == null)
                return null;
            else
                return dataSources.Values.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetDataSourceName(new Guid(context.EntityId.ToString()));
        }

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

            var adapterType = adapterTypes.FindRecord(x=>x.ExtensionConfigurationId == dataSource.AdapterTypeId);

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

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataSourcesUpdated(ref _updateHandle);
            }
        }


        #endregion
        
        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}
