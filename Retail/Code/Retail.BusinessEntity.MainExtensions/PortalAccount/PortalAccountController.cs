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
        public object GetPortalAccountSettings(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId)
        {
            if (!_manager.DosesUserHaveViewAccess(accountBEDefinitionId, accountViewDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
        }
        [HttpGet]
        [Route("GetPortalAccount")]
        public object GetPortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, int userId)
        {
            return _manager.GetPortalAccount(accountBEDefinitionId, accountId, accountViewDefinitionId, userId);
        }
        [HttpGet]
        [Route("GetPortalAccountDetails")]
        public List<PortalAccountDetail> GetPortalAccountDetails(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId)
        {
            return _manager.GetPortalAccountDetails(accountBEDefinitionId, accountId, accountViewDefinitionId);
        }
        [HttpGet]
        [Route("DosesUserHaveConfigureAccess")]
        public bool DosesUserHaveConfigureAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            return _manager.DosesUserHaveConfigureAccess(accountBEDefinitionId, accountViewDefinitionId);
        }

        [HttpPost]
        [Route("UpdatePortalAccount")]
        public object UpdatePortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            if (!DosesUserHaveConfigureAccess(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountViewDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdatePortalAccount(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountId, portalAccountEditorObject.AccountViewDefinitionId, portalAccountEditorObject.UserId,
                                             portalAccountEditorObject.Name, portalAccountEditorObject.Email);
        }
        [HttpPost]
        [Route("AddPortalAccount")]
        public object AddPortalAccount(PortalAccountEditorObject portalAccountEditorObject)
        {
            if (!DosesUserHaveConfigureAccess(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountViewDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.AddPortalAccount(portalAccountEditorObject.AccountBEDefinitionId, portalAccountEditorObject.AccountId, portalAccountEditorObject.AccountViewDefinitionId,
                                             portalAccountEditorObject.Name, portalAccountEditorObject.Email);
        }
        [HttpGet]
        [Route("DosesUserHaveResetPasswordAccess")]
        public bool DosesUserHaveAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            return _manager.DosesUserHaveResetPasswordAccess(accountBEDefinitionId, accountViewDefinitionId);
        }
        [HttpGet]
        [Route("EnablePortalAccount")]
        public object EnbalePortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId,long accountId,int userId)
        {
            return _manager.EnablePortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId);
        }
        [HttpGet]
        [Route("DisablePortalAccount")]
        public object DisablePortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId, long accountId, int userId)
        {
            return _manager.DisablePortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId);
        }
        [HttpGet]
        [Route("UnlockPortalAccount")]
        public object UnlockPortalAccount(Guid accountBEDefinitionId, Guid accountViewDefinitionId, long accountId, int userId)
        {
            return _manager.UnlockPortalAccount(accountBEDefinitionId, accountViewDefinitionId, accountId, userId);
        }
        [HttpPost]
        [Route("ResetPassword")]
        public object ResetPassword(ResetPasswordInput resetPasswordInput)
        {
            if (!_manager.DosesUserHaveResetPasswordAccess(resetPasswordInput.AccountBEDefinitionId, resetPasswordInput.AccountViewDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.ResetPassword(resetPasswordInput.AccountBEDefinitionId, resetPasswordInput.AccountId, resetPasswordInput.AccountViewDefinitionId, resetPasswordInput.Password, resetPasswordInput.UserId);
        }
    }

    public class PortalAccountEditorObject
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public Guid AccountViewDefinitionId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public int UserId { get; set; }
    }

    public class ResetPasswordInput
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public Guid AccountViewDefinitionId { get; set; }

        public string Password { get; set; }
        public int UserId { get; set; }
    }
    public class PortalAccountInput
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public Guid AccountViewDefinitionId { get; set; }

    }
}
