using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Integration.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataSource")]
    public class DataSourceController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDataSources")]
        public IEnumerable<Vanrise.Integration.Entities.DataSourceInfo> GetDataSources(string filter = null)
        {
            DataSourceManager manager = new DataSourceManager();
            DataSourceFilter deserializedFilter = Vanrise.Common.Serializer.Deserialize<DataSourceFilter>(filter);
            return manager.GetDataSources(deserializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredDataSources")]
        public object GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<DataSourceQuery> input)
        {
            DataSourceManager manager = new DataSourceManager();
            return GetWebResponse(input, manager.GetFilteredDataSources(input));
        }

        [HttpGet]
        [Route("GetDataSource")]
        public Vanrise.Integration.Entities.DataSourceDetail GetDataSource(Guid dataSourceId)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSourceDetail(dataSourceId);
        }

        [HttpGet]
        [Route("GetDataSourceAdapterTypes")]
        public IEnumerable<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSourceAdapterTypes();
        }

        [HttpGet]
        [Route("GetExecutionFlows")]
        public List<Vanrise.Queueing.Entities.QueueExecutionFlow> GetExecutionFlows()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetExecutionFlows();
        }

        [HttpPost]
        [Route("AddExecutionFlow")]
        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDetail> AddExecutionFlow(QueueExecutionFlow execFlowObject)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.AddExecutionFlow(execFlowObject);
        }

        [HttpGet]
        [Route("GetExecutionFlowDefinitions")]
        public List<QueueExecutionFlowDefinition> GetExecutionFlowDefinitions()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetExecutionFlowDefinitions();
        }

        [HttpPost]
        [Route("AddDataSource")]
        public Vanrise.Entities.InsertOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> AddDataSource(DataSourceWrapper dataSourceWrapper)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.AddDataSource(dataSourceWrapper.DataSourceData, dataSourceWrapper.TaskData);
        }

        [HttpPost]
        [Route("UpdateDataSource")]
        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Integration.Entities.DataSourceDetail> UpdateDataSource(DataSourceWrapper dataSourceWrapper)
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.UpdateDataSource(dataSourceWrapper.DataSourceData, dataSourceWrapper.TaskData);
        }

        [HttpGet]
        [Route("DeleteDataSource")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteDataSource(Guid dataSourceId, Guid taskId)
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