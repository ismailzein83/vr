using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;

namespace Vanrise.Integration.Web.Controllers
{
    public class DataSourceController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Vanrise.Integration.Entities.DataSource> GetDataSources()
        {
            DataSourceManager manager = new DataSourceManager();
            return manager.GetDataSources();
        }
    }
}