﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Analytic")]
    public class AnalyticController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRecords")]
        public Object GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            AnalyticManager manager = new AnalyticManager();
            return GetWebResponse(input, manager.GetFilteredRecords(input));
        }
        [HttpPost]
        [Route("GetRecordSearchFilterGroup")]
        public RecordFilterGroup GetRecordSearchFilterGroup(RecordSearchFilterGroupInput input)
        {
            AnalyticManager manager = new AnalyticManager();
            return manager.BuildRecordSearchFilterGroup(input);
        }
    }
}