using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class ProductServiceInfoFilter
    {
        public IEnumerable<IProductServiceFilter> Filters { get; set; }

    }
    
    public interface IProductServiceFilter
    {
        bool IsMatch(IProductServiceFilterContext context);
    }

    public interface IProductServiceFilterContext
    {
        ProductService ProductService { get; }
    }

    public class ProductServiceFilterContext : IProductServiceFilterContext
    {
        public ProductService ProductService { get; set; }
    }
}