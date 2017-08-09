using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ChangeStatusAction")]
    [JSONWithTypeAttribute]
    public class ChangeStatusActionController : BaseAPIController
    {
        ChangeStatusActionManager _manager = new ChangeStatusActionManager();
        [HttpGet]
        [Route("ChangeAccountStatus")]
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> ChangeAccountStatus(Guid accountBEDefinitionId, long accountId, Guid actionDefinitionId)
        {
            return _manager.ChangeAccountStatus(accountBEDefinitionId, accountId, actionDefinitionId);
        }
    }
}
