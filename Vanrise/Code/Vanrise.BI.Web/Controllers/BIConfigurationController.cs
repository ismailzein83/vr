using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BI.Entities;
using Vanrise.BI.Business;
using System.Net.Http;
using System.Web.Http;
namespace Vanrise.BI.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BIConfiguration")]
    public class BIConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetMeasuresInfo")]
        public IEnumerable<BIMeasureInfo> GetMeasuresInfo()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            return manager.GetMeasuresInfo();
        }

        [HttpGet]
        [Route("GetEntitiesInfo")]
        public IEnumerable<BIEntityInfo<BIConfigurationEntity>> GetEntitiesInfo()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            return manager.GetEntitiesInfo();
        }

        [HttpGet]
        [Route("GetTimeEntitiesInfo")]
        public IEnumerable<BIEntityInfo<BIConfigurationTimeEntity>> GetTimeEntitiesInfo()
        {
            BIConfigurationManager manager = new BIConfigurationManager();
            return manager.GetTimeEntitiesInfo();
        }
    }
}