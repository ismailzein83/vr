using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class ChangeUserRoutingGroupActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("51ECBE3E-CD99-4627-966E-E5D9A43E54EC"); }
        }
        public override string ClientActionName
        {
            get { return "ChangeUserRoutingGroup"; }
        }
        public Guid VRConnectionId { get; set; }

        public ChangeUserRoutingGroupActionSecurity Security { get; set; }

        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return DoesUserHaveExecutePermission();
        }

        public bool DoesUserHaveExecutePermission()
        {
            if (this.Security != null && this.Security.ExecutePermission != null)
                return ContextFactory.GetContext().IsAllowed(this.Security.ExecutePermission, ContextFactory.GetContext().GetLoggedInUserId());
            return true;
        }
    }
    public class ChangeUserRoutingGroupActionSecurity
    {
        public RequiredPermissionSettings ExecutePermission { get; set; }
    }
}
