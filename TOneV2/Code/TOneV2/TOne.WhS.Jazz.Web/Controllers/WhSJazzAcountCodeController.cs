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
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzAccountCode")]
    public class WhSJazzAccountCodeController:BaseAPIController
    {
        WhSJazzAccountCodeManager _manager = new WhSJazzAccountCodeManager();

        [HttpGet]
        [Route("GetTransctionType")]
        public GenericBusinessEntity GetTransctionType(Guid businessEntityDefinitionId, Guid trasctionTypeId)
        {
            return _manager.GetAccountCodeGenericBusinessEntity(trasctionTypeId, businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetAccountCodesInfo")]
        public IEnumerable<WhSJazzAccountCodeDetail> GetAccountCodesInfo(string filter = null)
        {
            WhSJazzAccountCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzAccountCodeInfoFilter>(filter) : null;
            return _manager.GetAccountCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllAccountCodes")]
        public IEnumerable<WhSJazzAccountCode> GetAllAccountCodes()
        {
            return _manager.GetAllAccountCodes();
        }
    }
}