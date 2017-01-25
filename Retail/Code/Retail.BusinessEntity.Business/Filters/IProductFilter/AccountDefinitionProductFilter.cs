using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountDefinitionProductFilter : IProductFilter
    {
        static ProductFamilyManager _productFamilyManager = new ProductFamilyManager();

        public Guid AccountBEDefinitionId { get; set; }

        public bool IsMatched(IProductFilterContext context)
        {
            ProductFamily productFamily = _productFamilyManager.GetProductFamily(context.Product.Settings.ProductFamilyId);

            if (context == null || context.Product == null || context.Product.Settings == null)
                return false;

            Guid productDefinitionAccountBEDefId = new ProductDefinitionManager().GetProductDefinitionAccountBEDefId(productFamily.Settings.ProductDefinitionId);
            if (this.AccountBEDefinitionId != productDefinitionAccountBEDefId)
                return false;

            return true;
        }
    }
}
