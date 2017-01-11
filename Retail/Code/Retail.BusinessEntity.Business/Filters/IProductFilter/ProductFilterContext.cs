using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class ProductFilterContext : IProductFilterContext
    {
        public Product Product { get; set; }
    }
}
