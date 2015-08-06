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

            // to be removed
            if (input.Query.From != null)
            {
                input.Query.From = input.Query.From.Value.AddHours(3);
                input.Query.From = input.Query.From.Value.AddSeconds(-input.Query.From.Value.Second);
                input.Query.From = input.Query.From.Value.AddMilliseconds(-input.Query.From.Value.Millisecond);
            }

            if (input.Query.To != null)
            {
                input.Query.To = input.Query.To.Value.AddHours(3);
                input.Query.To = input.Query.To.Value.AddSeconds(-input.Query.To.Value.Second);
                input.Query.To = input.Query.To.Value.AddMilliseconds(-input.Query.To.Value.Millisecond);
            }
            
            return GetWebResponse(input, manager.GetFilteredDataSourceLogs(input));
        }
    }
}