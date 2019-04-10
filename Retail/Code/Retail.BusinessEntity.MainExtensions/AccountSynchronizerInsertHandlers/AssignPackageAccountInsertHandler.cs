using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions.AccountSynchronizerInsertHandlers
{
    public enum CustomFieldType { ID = 0, Name = 1 }
    public class AssignPackageAccountInsertHandler : AccountSynchronizerInsertHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B5B45866-3B7B-4DDA-B893-F5E17E86EE1A"); }
        }
        public int AssignementDaysOffsetFromToday { get; set; }

        public DateTime? AssignementDate { get; set; }
        public int DefaultPackageId { get; set; }
        public string CustomFieldName { get; set; }
        public CustomFieldType CustomFieldType { get; set; }
        public override void OnPostInsert(IAccountSynchronizerInsertHandlerPostInsertContext context)
        {
            var account = context.Account;
            IAccountPayment financialPart;
            if (new AccountBEManager().HasAccountPayment(context.AccountBEDefinitionId, false, account, out financialPart))
            {

                if (financialPart.ProductId == default(int))
                    throw new NullReferenceException("financialPart.ProductId");
                IEnumerable<int> productPackages = new ProductManager().GetProductPackageIds(financialPart.ProductId);
                productPackages.ThrowIfNull("productPackages", financialPart.ProductId);

                DateTime assignementDate = this.AssignementDate.HasValue ? this.AssignementDate.Value : DateTime.Today.AddDays(-this.AssignementDaysOffsetFromToday);
                string packageName = null;
                int? packageId = null;
                if (context.CustomFields != null)
                {
                    if (CustomFieldName != null)
                    {
                        switch (CustomFieldType)
                        {
                            case CustomFieldType.ID:
                                packageId = (int)context.CustomFields.GetRecord(CustomFieldName);
                                break;
                            case CustomFieldType.Name:
                                packageName = context.CustomFields.GetRecord(CustomFieldName) as string;
                                break;
                        }
                    }
                }



                AccountPackageManager accountPackageManager = new AccountPackageManager();

                if (packageId.HasValue)
                {
                    if (!productPackages.Contains(packageId.Value))
                        throw new Exception(String.Format("Package '{0}' is not available in product '{1}'", packageId.Value, financialPart.ProductId));
                    accountPackageManager.AddAccountPackage(new AccountPackageToAdd
                    {
                        AccountBEDefinitionId = context.AccountBEDefinitionId,
                        AccountId = account.AccountId,
                        PackageId = packageId.Value,
                        BED = assignementDate
                    });
                    return;
                }
                if (packageName != null)
                {
                    var packages = new PackageManager().GetPackagesByIds(productPackages);
                    packages.ThrowIfNull("packages");

                    foreach (var package in packages)
                    {
                        if (package.Name.Trim().ToLower().Equals(packageName.Trim().ToLower()))
                        {
                            accountPackageManager.AddAccountPackage(new AccountPackageToAdd
                            {
                                AccountBEDefinitionId = context.AccountBEDefinitionId,
                                AccountId = account.AccountId,
                                PackageId = package.PackageId,
                                BED = assignementDate
                            });
                            return;
                        }
                    }
                }

                if (!productPackages.Contains(DefaultPackageId))
                {
                    throw new Exception(String.Format("Package '{0}' is not available in product '{1}'", DefaultPackageId, financialPart.ProductId));
                }
                accountPackageManager.AddAccountPackage(new AccountPackageToAdd
                {
                    AccountBEDefinitionId = context.AccountBEDefinitionId,
                    AccountId = account.AccountId,
                    PackageId = DefaultPackageId,
                    BED = assignementDate
                });
            }
        }
    }
}
