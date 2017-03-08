using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ProductFamilyAddOrEditFilter : IProductFamilyFilter
    {
        public bool EditMode { get; set; }
        public bool IsMatched(IProductFamilyFilterContext context)
        {
            ProductFamilyManager pFManager = new ProductFamilyManager();
            bool matched = (this.EditMode) ? pFManager.DoesUserHaveAddProductDefinitions(context.ProductFamilyId) || pFManager.DoesUserHaveEditProductDefinitions(context.ProductFamilyId) : pFManager.DoesUserHaveAddProductDefinitions(context.ProductFamilyId);
            return matched;
        }
    }
}
