using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class ProductInfoFilter
    {
        public List<IProductInfoFilter> Filters { get; set; }
    }
    public interface IProductInfoFilter
    {
        bool IsMatch(IProductInfoFilterContext context);
    }

    public interface IProductInfoFilterContext
    {
        long ProductId { get; set; }
    }
    public class ProductInfoFilterContext : IProductInfoFilterContext
    {
        public long ProductId { get; set; }
    }
}
