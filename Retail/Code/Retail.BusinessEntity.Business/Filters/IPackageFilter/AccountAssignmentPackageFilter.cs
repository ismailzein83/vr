using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountAssignmentPackageFilter : IPackageFilter
    {
        static AccountPackageManager s_accountPackageManager = new AccountPackageManager();

        public Guid AccountBEDefinitionId { get; set; }
        public long AssignedToAccountId { get; set; }

        public bool IsMatched(IPackageFilterContext context)
        {
            context.Package.ThrowIfNull("context.Package");
            return s_accountPackageManager.CanAssignPackageToAccount(context.Package, this.AccountBEDefinitionId, this.AssignedToAccountId);
        }
    }
}
