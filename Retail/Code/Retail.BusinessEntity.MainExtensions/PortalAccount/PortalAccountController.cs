using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.MainExtensions.PortalAccount
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
            return _manager.AddPortalAccount(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountId, portalAccountEditorObject.AccountViewDefinitionId,
                                             portalAccountEditorObject.Name, portalAccountEditorObject.Email);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(ResetPasswordInput resetPasswordInput)
        {
            return _manager.ResetPassword(resetPasswordInput.AccountBEDefinitionId, resetPasswordInput.AccountId, resetPasswordInput.AccountViewDefinitionId, resetPasswordInput.Password);
        }
    }

    public class PortalAccountEditorObject
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public Guid AccountViewDefinitionId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }

    public class ResetPasswordInput
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public Guid AccountViewDefinitionId { get; set; }

        public string Password { get; set; }
    }
}
