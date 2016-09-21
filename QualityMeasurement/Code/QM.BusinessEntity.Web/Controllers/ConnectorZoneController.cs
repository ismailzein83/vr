using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace QM.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ConnectorZone")]

    public class QMBE_ConnectorZoneController : BaseAPIController
    {
        [HttpGet]
        [Route("GetConnectorZoneTemplates")]
        public IEnumerable<ConnectorZoneReaderConfig> GetConnectorZoneTemplates()
        {
            ConnectorZoneInfoManager manager = new ConnectorZoneInfoManager();
            return manager.GetConnectorZoneTemplates();
        }
    }
}