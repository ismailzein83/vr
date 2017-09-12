using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class MappingTelesUserActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("74384112-0D3E-4677-8DF3-2C16C32A84D4"); }
        }
        public override string ClientActionName
        {
            get { return "MappingTelesUser"; }
        }
        public Guid VRConnectionId { get; set; }
        public MappingTelesUserActionSecurity Security { get; set; }
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
    public class MappingTelesUserActionSecurity
    {
        public RequiredPermissionSettings ExecutePermission { get; set; }
    }
}
