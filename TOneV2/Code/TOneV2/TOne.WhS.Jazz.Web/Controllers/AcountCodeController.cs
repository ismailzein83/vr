using System;
using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountCode")]
    public class AccountCodeController:BaseAPIController
    {
        AccountCodeManager _manager = new AccountCodeManager();

        [HttpGet]
        [Route("GetTransctionType")]
        public GenericBusinessEntity GetTransctionType(Guid businessEntityDefinitionId, Guid trasctionTypeId)
        {
            return _manager.GetAccountCodeGenericBusinessEntity(trasctionTypeId, businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetAccountCodesInfo")]
        public IEnumerable<AccountCodeDetail> GetAccountCodesInfo(string filter = null)
        {
            AccountCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<AccountCodeInfoFilter>(filter) : null;
            return _manager.GetAccountCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllAccountCodes")]
        public IEnumerable<AccountCode> GetAllAccountCodes()
        {
            return _manager.GetAllAccountCodes();
        }
    }
}