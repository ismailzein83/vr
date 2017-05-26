using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RecordFilter")]
    public class RecordFilterController:BaseAPIController
    {
        [HttpPost]
        [Route("BuildRecordFilterGroupExpression")]
        public string BuildRecordFilterGroupExpression(RecordFilterGroupExpressionInput input)
        {
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            return recordFilterManager.BuildRecordFilterGroupExpression(input.FilterGroup, input.RecordFilterFieldInfosByFieldName);
        }
    }
}