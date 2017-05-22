using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Entities.Settings;
using Vanrise.Web.Base;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DealDefinition")]
    public class DealDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDealDefinitionInfo")]
        public IEnumerable<DealDefinitionInfo> GetDealDefinitionInfo(string serializedFilter = null)
        {
            var manager = new DealDefinitionManager();
            var deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<DealDefinitionFilter>(serializedFilter) : null;
            return manager.GetDealDefinitionInfo(deserializedFilter);
        }
    }
}