using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountSynchronizerInsertHandlers
{
    public class AssignProductAndPackagesAccountInsertHandler : AccountSynchronizerInsertHandlerSettings
    {
        public int ProductId { get; set; }

        public List<int> Packages { get; set; }

        public int AssignementDaysOffsetFromToday { get; set; }

        public DateTime? AssignementDate { get; set; }

        public override void OnPreInsert(IAccountSynchronizerInsertHandlerPreInsertContext context)
        {
            var account = context.Account;
            IAccountPayment financialPart;
            if (new AccountBEManager().HasAccountPayment(context.AccountBEDefinitionId, false, account, out financialPart))
                financialPart.ProductId = this.ProductId;
        }

        public override void OnPostInsert(IAccountSynchronizerInsertHandlerPostInsertContext context)
        {
            var account = context.Account;
            if (this.Packages != null)
            {
                IAccountPayment financialPart;
                if (new AccountBEManager().HasAccountPayment(context.AccountBEDefinitionId, false, account, out financialPart))
                {
                    ValidatePackages(financialPart);
                    AccountPackageManager accountPackageManager = new AccountPackageManager();
                    DateTime assignementDate = this.AssignementDate.HasValue ? this.AssignementDate.Value : DateTime.Today.AddDays(-this.AssignementDaysOffsetFromToday);
                    foreach (var packageId in this.Packages)
                    {
                        accountPackageManager.AddAccountPackage(new AccountPackageToAdd
                        {
                            AccountBEDefinitionId = context.AccountBEDefinitionId,
                            AccountId = account.AccountId,
                            PackageId = packageId,
                            BED = assignementDate
                        });
                    }
                }
            }
        }

        private void ValidatePackages(IAccountPayment financialPart)
        {
            if (financialPart.ProductId == default(int))
                throw new NullReferenceException("financialPart.ProductId");
            IEnumerable<int> productPackages = new ProductManager().GetProductPackageIds(financialPart.ProductId);
            foreach (var packageId in this.Packages)
            {
                if (productPackages == null || !productPackages.Contains(packageId))
                {
                    throw new Exception(String.Format("Package '{0}' is not available in product '{1}'", packageId, financialPart.ProductId));
                }
            }
        }
    }
}
