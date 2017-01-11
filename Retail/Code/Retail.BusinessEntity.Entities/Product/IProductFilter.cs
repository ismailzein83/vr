using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IProductFilter
    {
        bool IsMatched(IProductFilterContext context);
    }

    public interface IProductFilterContext
    {
        Product Product { get; }
    }
}
