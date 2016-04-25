﻿using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DataRecordFields")]
    public class DataRecordFieldsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordFieldsInfo")]
        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(string serializedFilter)
        {
            DataRecordFieldInfoFilter filter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<DataRecordFieldInfoFilter>(serializedFilter) : null;
            DataRecordFieldManager manager = new DataRecordFieldManager();
            return manager.GetDataRecordFieldsInfo(filter);
        }
    }
}