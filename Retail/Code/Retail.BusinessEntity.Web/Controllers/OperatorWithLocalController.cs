using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorsWithLocal")]
    public class OperatorWithLocalController : BaseAPIController
    {
        private OperatorsWithLocalManager _operatorWithLocalManager = new OperatorsWithLocalManager();

        [HttpGet]
        [Route("GetOperatorsWithLocalInfo")]
        public IEnumerable<AccountInfo> GetOperatorsWithLocalInfo(Guid businessEntityDefinitionId)
        {
            return _operatorWithLocalManager.GetOperatorsWithLocal(businessEntityDefinitionId);
        }
    }
}