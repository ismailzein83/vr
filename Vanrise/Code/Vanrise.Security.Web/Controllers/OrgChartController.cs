using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class OrgChartController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public IEnumerable<OrgChart> GetOrgCharts()
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.GetOrgCharts();
        }

        [HttpGet]
        public IEnumerable<OrgChart> GetFilteredOrgCharts(int fromRow, int toRow, string name)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.GetFilteredOrgCharts(fromRow, toRow, name);
        }
        
        [HttpGet]
        public OrgChart GetOrgChartById(int orgChartId)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.GetOrgChartById(orgChartId);
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<OrgChart> AddOrgChart(OrgChart orgChartObject)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.AddOrgChart(orgChartObject);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<OrgChart> UpdateOrgChart(OrgChart orgChartObject)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.UpdateOrgChart(orgChartObject);
        }

        [HttpGet]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteOrgChart(int orgChartId)
        {
            OrgChartManager manager = new OrgChartManager();
            return manager.DeleteOrgChart(orgChartId);
        }
    }
}
