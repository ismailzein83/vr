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
        [HttpPost]
        [Route("GetFilteredOrgCharts")]
        public object GetFilteredOrgCharts(Vanrise.Entities.DataRetrievalInput<OrgChartQuery> input)
        {
            OrgChartManager manager = new OrgChartManager();
            return GetWebResponse(input, manager.GetFilteredOrgCharts(input));
        }
        
        [HttpGet]
        [Route("GetOrgChartById")]
        public OrgChart GetOrgChartById(int orgChartId)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.GetOrgChartById(orgChartId);
        }

        [HttpPost]
        [Route("AddOrgChart")]
        public Vanrise.Entities.InsertOperationOutput<OrgChart> AddOrgChart(OrgChart orgChartObject)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.AddOrgChart(orgChartObject);
        }

        [HttpPost]
        [Route("UpdateOrgChart")]
        public Vanrise.Entities.UpdateOperationOutput<OrgChart> UpdateOrgChart(OrgChart orgChartObject)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.UpdateOrgChart(orgChartObject);
        }

        [HttpGet]
        [Route("DeleteOrgChart")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.DeleteOrgChart(orgChartId);
        }
    }
}
