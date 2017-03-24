using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class MappingTelesAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("09A1029D-AFC0-48E4-B1B3-FD951462E267"); }
        }

        public override string ClientActionName
        {
            get { return "MappingTelesAccount"; }
        }
        public Guid VRConnectionId { get; set; }

        public MappingTelesAccountActionSecurity Security { get; set; }

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

    public class MappingTelesAccountActionSecurity
    {
        public RequiredPermissionSettings ExecutePermission { get; set; }
    }
}
