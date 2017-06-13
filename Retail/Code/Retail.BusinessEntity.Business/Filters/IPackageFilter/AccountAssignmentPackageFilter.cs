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
        static AccountBEManager _accountBEManager = new AccountBEManager();

        public Guid AccountBEDefinitionId { get; set; }
        public long AssignedToAccountId { get; set; }

        public bool IsMatched(IPackageFilterContext context)
        {
            context.Package.ThrowIfNull("context.Package");
            context.Package.Settings.ThrowIfNull("context.Package.Settings");
            context.Package.Settings.ExtendedSettings.ThrowIfNull("context.Package.Settings.ExtendedSettings");
            var canAssignContext = new PackageSettingsCanAssignPackageContext
            {
                AccountDefinitionId = this.AccountBEDefinitionId,
                AccountId = this.AssignedToAccountId,
                Package = context.Package
            };
            return context.Package.Settings.ExtendedSettings.CanAssignPackage(canAssignContext);
        }

        public static bool IsPackageAvailableInAccountProduct(Guid accountBEDefinitionId, long accountId, int packageId)
        {
            IAccountPayment accountPayment;

            _accountBEManager.HasAccountPayment(accountBEDefinitionId, accountId, true, out accountPayment);
            if (accountPayment == null)
                throw new NullReferenceException(string.Format("accountPayment for accountId {0}", accountId));
            IEnumerable<int> packageIds = new ProductManager().GetProductPackageIds(accountPayment.ProductId);

            if (packageIds == null || !packageIds.Contains(packageId))
                return false;

            //IEnumerable<int> packageIdsAssignedToAccount = new AccountPackageManager().GetPackageIdsAssignedToAccount(this.AssignedToAccountId);
            //if (packageIdsAssignedToAccount != null && packageIdsAssignedToAccount.Contains(context.Package.PackageId))
            //    return false;

            return true;
        }

        private class PackageSettingsCanAssignPackageContext : IPackageSettingsCanAssignPackageContext
        {
            public Package Package
            {
                get;
                set;
            }

            public Guid AccountDefinitionId
            {
                get;
                set;
            }

            public long AccountId
            {
                get;
                set;
            }
        }

    }
}
