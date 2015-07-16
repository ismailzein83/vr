using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
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

    }

    public class DataSourceWrapper
    {
        public Vanrise.Integration.Entities.DataSource DataSourceData { get; set; }

        public Vanrise.Runtime.Entities.SchedulerTask TaskData { get; set; }
    }
}