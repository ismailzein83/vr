using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamilySettings
    {
        public Guid ProductDefinitionId { get; set; }

        public Dictionary<int, ProductPackageItem> Packages { get; set; }
    }
}
