using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamilyFilter
    {
        public List<IProductFamilyFilter> Filters { get; set; }
    }

    public interface IProductFamilyFilter
    {
        bool IsMatched(IProductFamilyFilterContext context);
    }

    public interface IProductFamilyFilterContext
    {
        int ProductFamilyId { get; }
    }

    public class ProductFamilyFilterContext : IProductFamilyFilterContext
    {
        public int ProductFamilyId { get; set; }
    }
}
