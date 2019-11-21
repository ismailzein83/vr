using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class UnmappingTelesAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get
            {
                return new Guid("C7658EFB-EC75-4BFD-94C9-0B7EFA7EB158");
            }
        }
        public override string ClientActionName
        {
            get { return "UnmappingTelesAccount"; }
        }

        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public Guid UserTypeId { get; set; }
        public UnmappingTelesAccountActionSecurity Security { get; set; }

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

    public class UnmappingTelesAccountActionSecurity
    {
        public RequiredPermissionSettings ExecutePermission { get; set; }
    }
}
