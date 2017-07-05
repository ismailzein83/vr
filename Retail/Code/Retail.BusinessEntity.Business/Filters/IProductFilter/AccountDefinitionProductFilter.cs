using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountDefinitionProductFilter : IProductFilter
    {
        static ProductFamilyManager s_productFamilyManager = new ProductFamilyManager();
        static ProductManager s_productManager = new ProductManager();
        static AccountBEManager s_accountManager = new AccountBEManager();

        public Guid AccountBEDefinitionId { get; set; }

        public long? AccountId { get; set; }

        public bool IsMatched(IProductFilterContext context)
        {
            context.Product.ThrowIfNull("context.Product");
            context.Product.Settings.ThrowIfNull("context.Product.Settings");
            ProductFamily productFamily = s_productFamilyManager.GetProductFamily(context.Product.Settings.ProductFamilyId);
            productFamily.ThrowIfNull("productFamily", context.Product.Settings.ProductFamilyId);
            productFamily.Settings.ThrowIfNull("productFamily.Settings", productFamily.ProductFamilyId);

            Guid productDefinitionAccountBEDefId = new ProductDefinitionManager().GetProductDefinitionAccountBEDefId(productFamily.Settings.ProductDefinitionId);
            if (this.AccountBEDefinitionId != productDefinitionAccountBEDefId)
                return false;

            if(this.AccountId.HasValue)
            {
                IAccountPayment accountPayment;
                if(s_accountManager.HasAccountPayment(this.AccountBEDefinitionId, this.AccountId.Value, false, out accountPayment))
                {
                    int? existingProductFamilyId = s_productManager.GetProductFamilyId(accountPayment.ProductId);
                    if (existingProductFamilyId.HasValue && productFamily.ProductFamilyId != existingProductFamilyId.Value)
                        return false;
                }
            }

            return true;
        }
    }
}
