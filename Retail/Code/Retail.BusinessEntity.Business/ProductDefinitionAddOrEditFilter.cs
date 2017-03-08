using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ProductDefinitionAddOrEditFilter : IProductDefinitionFilter
    {
        public bool EditMode { get; set; }
        public bool IsMatched(IProductDefinitionFilterContext context)
        {
            ProductDefinitionManager pdmanager = new ProductDefinitionManager();
            bool matched = (this.EditMode) ? pdmanager.DoesUserHaveAddProductDefinitions(context.ProductDefinitionId) || pdmanager.DoesUserHaveEditProductDefinitions(context.ProductDefinitionId) : pdmanager.DoesUserHaveAddProductDefinitions(context.ProductDefinitionId);
            return matched;
        }
    }
}
