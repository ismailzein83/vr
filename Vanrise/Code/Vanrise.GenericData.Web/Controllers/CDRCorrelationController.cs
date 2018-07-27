using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRCorrelation")]

    public class CDRCorrelationController: BaseAPIController
    {
        CDRCorrelationDefinitionManager _manager = new CDRCorrelationDefinitionManager();

        [HttpGet]
        [Route("GetCDRCorrelationDefinitionsInfo")]
        public IEnumerable<CDRCorrelationDefinitionInfo> GetCDRCorrelationDefinitionsInfo(string filter = null)
        {
            CDRCorrelationDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<CDRCorrelationDefinitionInfoFilter>(filter) : null;
            return _manager.GetCDRCorrelationDefinitionsInfo(deserializedFilter);
        }
    }
}