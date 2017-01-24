using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamily
    {
        public int ProductFamilyId { get; set; }

        public string Name { get; set; }

        public ProductFamilySettings Settings { get; set; }
    }
}
