using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Tariff")]
    [JSONWithTypeAttribute]
    public class TariffController : BaseAPIController
    {

        TariffManager _manager = new TariffManager();

        [HttpGet]
        [Route("GetTariffsInfo")]
        public IEnumerable<TariffInfo> GetTariffsInfo(string filter = null)
        {
            TariffFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<TariffFilter>(filter) : null;
            return _manager.GetTariffsInfo(deserializedFilter);
        } 
    }
}