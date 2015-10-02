using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceManager : IDataSourceManager
    {
        public List<Vanrise.Integration.Entities.DataSourceInfo> GetDataSources()
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSources();
        }

        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Integration.Entities.DataSource> GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<DataSourceQuery> input)
        {
            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDataSources(input));
        }

        public Vanrise.Integration.Entities.DataSource GetDataSource(int dataSourceId)
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSource(dataSourceId);
        }

        public Vanrise.Integration.Entities.DataSource GetDataSourcebyTaskId(int taskId)
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSourcebyTaskId(taskId);
        }

        public List<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            IDataSourceAdapterTypeDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceAdapterTypeDataManager>();
            return datamanager.GetDataSourceAdapterTypes();
        }

        public Vanrise.Entities.InsertOperationOutput<Vanrise.Integration.Entities.DataSource> AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject, 
            Vanrise.Runtime.Entities.SchedulerTask taskObject)
        {
            InsertOperationOutput<Vanrise.Integration.Entities.DataSource> insertOperationOutput = new InsertOperationOutput<Vanrise.Integration.Entities.DataSource>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int taskId;
            Vanrise.Runtime.Data.ISchedulerTaskDataManager taskDataManager = Vanrise.Runtime.Data.RuntimeDataManagerFactory.GetDataManager<Vanrise.Runtime.Data.ISchedulerTaskDataManager>();
            bool taskInsertActionSucc = taskDataManager.AddTask(taskObject, out taskId);
            
            if(taskInsertActionSucc)
            {
                int dataSourceId = -1;

                dataSourceObject.TaskId = taskId;
                
                IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool dataSourceInsertActionSucc = dataManager.AddDataSource(dataSourceObject, out dataSourceId);

                if (dataSourceInsertActionSucc)
                {
                    insertOperationOutput.Result = InsertOperationResult.Succeeded;
                    dataSourceObject.DataSourceId = dataSourceId;
                    insertOperationOutput.InsertedObject = dataSourceObject;
                }
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSource> UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject,
             Vanrise.Runtime.Entities.SchedulerTask taskObject)
        {
            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();

            bool dataSourceUpdateActionSucc = dataManager.UpdateDataSource(dataSourceObject);
            UpdateOperationOutput<Vanrise.Integration.Entities.DataSource> updateOperationOutput = new UpdateOperationOutput<Vanrise.Integration.Entities.DataSource>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            Vanrise.Runtime.Data.ISchedulerTaskDataManager taskDataManager = Vanrise.Runtime.Data.RuntimeDataManagerFactory.GetDataManager<Vanrise.Runtime.Data.ISchedulerTaskDataManager>();
            bool taskUpdateActionSucc = taskDataManager.UpdateTaskInfo(taskObject.TaskId, taskObject.Name, taskObject.IsEnabled, 
                taskObject.TriggerTypeId, taskObject.ActionTypeId, taskObject.TaskSettings);

            if (taskUpdateActionSucc)
            {
                if (dataSourceUpdateActionSucc)
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = dataSourceObject;
                }
            }
            
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteDataSource(int dataSourceId, int taskId)
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

        public Vanrise.Entities.UpdateOperationOutput<object> AddDataSourceTask(int dataSourceId, Vanrise.Runtime.Entities.SchedulerTask task)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            int taskId;
            Vanrise.Runtime.Data.ISchedulerTaskDataManager taskDataManager = Vanrise.Runtime.Data.RuntimeDataManagerFactory.GetDataManager<Vanrise.Runtime.Data.ISchedulerTaskDataManager>();
            bool insertActionSucc = taskDataManager.AddTask(task, out taskId);

            if (insertActionSucc)
            {
                IDataSourceDataManager dataSourceDataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
                bool updateActionSucc = dataSourceDataManager.UpdateTaskId(dataSourceId, taskId);
                
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

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlow> AddExecutionFlow(QueueExecutionFlow execFlowObject)
        {
            QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
            return manager.AddExecutionFlow(execFlowObject);
        }

        public List<QueueExecutionFlowDefinition> GetExecutionFlowDefinitions()
        {
            QueueExecutionFlowDefinitionManager manager = new QueueExecutionFlowDefinitionManager();
            return manager.GetAll();
        }

        public bool UpdateAdapterState(int dataSourceId, Vanrise.Integration.Entities.BaseAdapterState adapterState)
        {
            IDataSourceDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return manager.UpdateAdapterState(dataSourceId, adapterState);
        }
    }
}
