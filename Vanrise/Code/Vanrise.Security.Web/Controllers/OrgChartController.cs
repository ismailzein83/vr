using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OrgChart")]
    public class OrgChartController : Vanrise.Web.Base.BaseAPIController
    {
        OrgChartManager _manager;
        public OrgChartController()
        {
            _manager = new OrgChartManager();
        }

        [HttpPost]
        [Route("GetFilteredOrgCharts")]
        public object GetFilteredOrgCharts(Vanrise.Entities.DataRetrievalInput<OrgChartQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredOrgCharts(input), "Org Charts");
        }

        [HttpGet]
        [Route("GetOrgChartInfo")]
        public IEnumerable<OrgChartInfo> GetOrgChartInfo()
        {
            return _manager.GetOrgChartInfo();
        }
        
        [HttpGet]
        [Route("GetOrgChartById")]
        public OrgChart GetOrgChartById(int orgChartId)
        {
            return _manager.GetOrgChartById(orgChartId,true);
        }

        [HttpPost]
        [Route("AddOrgChart")]
        public Vanrise.Entities.InsertOperationOutput<OrgChart> AddOrgChart(OrgChart orgChartObject)
        {
            return _manager.AddOrgChart(orgChartObject);
        }

        [HttpPost]
        [Route("UpdateOrgChart")]
        public Vanrise.Entities.UpdateOperationOutput<OrgChart> UpdateOrgChart(OrgChart orgChartObject)
        {
            return _manager.UpdateOrgChart(orgChartObject);
        }

        [HttpGet]
        [Route("DeleteOrgChart")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            return _manager.DeleteOrgChart(orgChartId);
        }
    }
}
