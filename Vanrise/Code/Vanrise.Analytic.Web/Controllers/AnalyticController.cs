﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
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
            if (manager.CheckAnalyticRequiredPermission(input))

                return GetWebResponse(input, manager.GetFilteredRecords(input),string.IsNullOrEmpty(input.Query.ReportTitle)? input.Query.ReportName: input.Query.ReportTitle);

            return GetUnauthorizedResponse();
          
        }
        [HttpPost]
        [Route("GetRecordSearchFilterGroup")]
        public RecordFilterGroup GetRecordSearchFilterGroup(RecordSearchFilterGroupInput input)
        {
            AnalyticManager manager = new AnalyticManager();
            return manager.BuildRecordSearchFilterGroup(input);
        }
        [HttpPost]
        [Route("GetRecordSearchFieldFilter")]
        public RecordFilterGroup GetRecordSearchFieldFilter(RecordSearchFieldFilterInput input)
        {
            AnalyticManager manager = new AnalyticManager();
            return manager.BuildRecordSearchFieldFilter(input);
        }

        [HttpPost]
        [Route("GetMeasureStyleRulesRanges")]
        public List<VRVisualizationRange> GetMeasureStyleRulesRanges(MeasureStyleRangeInput input)
        {
            AnalyticManager manager = new AnalyticManager();
            return manager.GetMeasureStyleRulesRanges(input.MeasureName, input.AnalyticTableId);
        }
    }
    public class MeasureStyleRangeInput
    {
        public string MeasureName { get; set; }
        public Guid AnalyticTableId { get; set; }
    }
}