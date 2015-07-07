using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Integration.Data;

namespace Vanrise.Integration.Business
{
    public class DataSourceManager
    {
        public List<Vanrise.Integration.Entities.DataSource> GetDataSources()
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSources();
        }

        public Vanrise.Integration.Entities.DataSource GetDataSource(int dataSourceId)
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSource(dataSourceId);
        }

        public List<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            IDataSourceAdapterTypeDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceAdapterTypeDataManager>();
            return datamanager.GetDataSourceAdapterTypes();
        }

        public Vanrise.Entities.InsertOperationOutput<Vanrise.Integration.Entities.DataSource> AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject)
        {
            InsertOperationOutput<Vanrise.Integration.Entities.DataSource> insertOperationOutput = new InsertOperationOutput<Vanrise.Integration.Entities.DataSource>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataSourceId = -1;

            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            bool insertActionSucc = dataManager.AddDataSource(dataSourceObject, out dataSourceId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                dataSourceObject.DataSourceId = dataSourceId;
                insertOperationOutput.InsertedObject = dataSourceObject;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSource> UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject)
        {
            IDataSourceDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();

            bool updateActionSucc = dataManager.UpdateDataSource(dataSourceObject);
            UpdateOperationOutput<Vanrise.Integration.Entities.DataSource> updateOperationOutput = new UpdateOperationOutput<Vanrise.Integration.Entities.DataSource>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = dataSourceObject;
            }
            return updateOperationOutput;
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
    }
}
