using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBESerialNumber")]
    public class GenericBESerialNumberController:BaseAPIController
    {

        [HttpGet]
        [Route("GetSerialNumberPartDefinitionsInfo")]
        public IEnumerable<GenericBESerialNumberPartInfo> GetSerialNumberPartDefinitionsInfo(Guid businessEntityDefinitionId)
        {
            GenericBESerialNumberManager manager = new GenericBESerialNumberManager();
            return manager.GetSerialNumberPartDefinitionsInfo(businessEntityDefinitionId);
        }

    }
}
