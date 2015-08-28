using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Queueing.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Integration.Web.Controllers
{
    [JSONWithTypeAttribute]
    public class DataSourceController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Vanrise.Integration.Entities.DataSource> GetDataSources()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSources();
        }

        [HttpPost]
        public object GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<object> input)
        {
            DataSourceManager manager = new DataSourceManager();
            return GetWebResponse(input, manager.GetFilteredDataSources(input));
        }

        [HttpGet]
        public Vanrise.Integration.Entities.DataSource GetDataSource(int dataSourceId)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSource(dataSourceId);
        }

        [HttpGet]
        public List<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSourceAdapterTypes();
        }

        [HttpGet]
        public List<Vanrise.Queueing.Entities.QueueExecutionFlow> GetExecutionFlows()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetExecutionFlows();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlow> AddExecutionFlow(QueueExecutionFlow execFlowObject)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.AddExecutionFlow(execFlowObject);
        }

        [HttpGet]
        public List<QueueExecutionFlowDefinition> GetExecutionFlowDefinitions()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetExecutionFlowDefinitions();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<Vanrise.Integration.Entities.DataSource> AddDataSource(DataSourceWrapper dataSourceWrapper)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.AddDataSource(dataSourceWrapper.DataSourceData, dataSourceWrapper.TaskData);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSource> UpdateDataSource(DataSourceWrapper dataSourceWrapper)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.UpdateDataSource(dataSourceWrapper.DataSourceData, dataSourceWrapper.TaskData);
        }

        [HttpGet]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteDataSource(int dataSourceId, int taskId)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.DeleteDataSource(dataSourceId, taskId);
        }
    }

    public class DataSourceWrapper
    {
        public Vanrise.Integration.Entities.DataSource DataSourceData { get; set; }

        public Vanrise.Runtime.Entities.SchedulerTask TaskData { get; set; }
    }
}