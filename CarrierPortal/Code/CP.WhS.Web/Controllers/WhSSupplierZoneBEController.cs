using CP.WhS.Business;
using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.APIEntities;
using Vanrise.Common;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSSupplierZoneBE")]
    [JSONWithTypeAttribute]
    public class WhSSupplierZoneBEController : BaseAPIController
    {
        WhSSupplierZoneBEManager _whSSupplierZoneBEManager = new WhSSupplierZoneBEManager();
        [HttpGet]
        [Route("GetRemoteSupplierZonesInfo")]
        public IEnumerable<ClientSupplierZoneInfo> GetRemoteSupplierZonesInfo(string serializedFilter)
        {
            return _whSSupplierZoneBEManager.GetRemoteSupplierZonesInfo(Vanrise.Common.Serializer.Deserialize<ClientSupplierZoneInfoFilter>(serializedFilter));
        }
    }
}