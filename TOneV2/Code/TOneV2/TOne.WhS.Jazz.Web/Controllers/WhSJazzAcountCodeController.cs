using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Entities;
using TOne.WhS.Jazz.Web;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Business;

namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzAccountCode")]
    public class WhSJazzAccountCodeController:BaseAPIController
    {
        WhsJazzAccountCodeManager _manager = new WhsJazzAccountCodeManager();

        [HttpGet]
        [Route("GetTransctionType")]
        public GenericBusinessEntity GetTransctionType(Guid businessEntityDefinitionId, Guid trasctionTypeId)
        {
            return _manager.GetAccountCodeGenericBusinessEntity(trasctionTypeId, businessEntityDefinitionId);
        }

    }
}