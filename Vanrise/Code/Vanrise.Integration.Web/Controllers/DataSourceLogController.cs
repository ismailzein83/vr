using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Integration.Business;
using Vanrise.Web.Base;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Web.Controllers
{
    public class DataSourceLogController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            DataSourceLogManager manager = new DataSourceLogManager();            
            return GetWebResponse(input, manager.GetFilteredDataSourceLogs(input));
        }
    }
}