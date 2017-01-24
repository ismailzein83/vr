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
        public Guid AccountBEDefinitionId { get; set; }

        public bool IsMatched(IProductFilterContext context)
        {
            //Guid productDefinitionAccountBEDefId;

            //if (context != null && context.Product != null && context.Product.Settings != null)
            //{
            //    productDefinitionAccountBEDefId = new ProductDefinitionManager().GetProductDefinitionAccountBEDefId(context.Product.Settings.ProductDefinitionId);

            //    if (this.AccountBEDefinitionId != productDefinitionAccountBEDefId)
            //        return false;
            //}

            return true;
        }
    }
}
