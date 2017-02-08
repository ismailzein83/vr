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
    [RoutePrefix(Constants.ROUTE_PREFIX + "PortalAccount")]
    [JSONWithTypeAttribute]
    public class PortalAccountController : BaseAPIController
    {
        PortalAccountManager _manager = new PortalAccountManager();

        [HttpGet]
        [Route("GetPortalAccountSettings")]
        public PortalAccountSettings GetPortalAccountSettings(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetPortalAccountSettings(accountBEDefinitionId, accountId);
        }

        [HttpPost]
        [Route("AddPortalAccount")]
        public Vanrise.Entities.InsertOperationOutput<PortalAccountSettings> AddPortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            return _manager.AddPortalAccount(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountId, portalAccountEditorObject.Name, portalAccountEditorObject.Email, 
                                             portalAccountEditorObject.ConnectionId, portalAccountEditorObject.TenantId);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(ResetPasswordInput resetPasswordInput)
        {
            return _manager.ResetPassword(resetPasswordInput.UserId, resetPasswordInput.Password, resetPasswordInput.ConnectionId);
        }
    }

    public class PortalAccountEditorObject
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Guid ConnectionId { get; set; }

        public int TenantId { get; set; }
    }

    public class ResetPasswordInput
    {
        public int UserId {get; set;}

        public string Password {get; set;}

        public Guid ConnectionId { get; set; }
    }
}