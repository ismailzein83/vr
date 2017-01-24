using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamilyInfo
    {
        public int ProductFamilyId { get; set; }

        public string Name { get; set; }

        public Guid ProductDefinitionId { get; set; }
    }
}
