﻿using Vanrise.Tools.Business;
using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;


namespace Vanrise.Tools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Columns")]
    [JSONWithTypeAttribute]
    public class ColumnsController : BaseAPIController
    {
        ColumnsManager columnsManager = new ColumnsManager();
       
       
        [HttpGet]
        [Route("GetColumnsInfo")]
        public IEnumerable<ColumnsInfo> GetColumnsInfo(string filter = null)
        {
            ColumnsInfoFilter columnsInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ColumnsInfoFilter>(filter) : null;
            return columnsManager.GetColumnsInfo(columnsInfoFilter);
        }
        

    }
}