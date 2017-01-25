using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountAssignmentPackageFilter : IPackageFilter
    {
        static AccountBEManager _accountBEManager = new AccountBEManager();

        public Guid AccountBEDefinitionId { get; set; }
        public long AssignedToAccountId { get; set; }

        public bool IsMatched(IPackageFilterContext context)
        {
            IAccountPayment accountPayment;

            _accountBEManager.HasAccountPayment(this.AccountBEDefinitionId, this.AssignedToAccountId, true, out accountPayment);
            if (accountPayment == null)
                throw new NullReferenceException(string.Format("accountPayment for accountId {0}", this.AssignedToAccountId));
            IEnumerable<int> packageIds = new ProductManager().GetProductPackageIds(accountPayment.ProductId);

            if (packageIds == null || !packageIds.Contains(context.Package.PackageId))
                return false;

            //IEnumerable<int> packageIdsAssignedToAccount = new AccountPackageManager().GetPackageIdsAssignedToAccount(this.AssignedToAccountId);
            //if (packageIdsAssignedToAccount != null && packageIdsAssignedToAccount.Contains(context.Package.PackageId))
            //    return false;

            return true;
        }
    }
}
