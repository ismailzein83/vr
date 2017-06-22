using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class MappingTelesSiteActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("638C3DCC-F05A-4FA5-83BB-5E24CB2DA9C8"); }
        }

        public override string ClientActionName
        {
            get { return "MappingTelesSite"; }
        }
        public Guid VRConnectionId { get; set; }

        public MappingTelesSiteActionSecurity Security { get; set; }

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

    public class MappingTelesSiteActionSecurity
    {
        public RequiredPermissionSettings ExecutePermission { get; set; }
    }
}
