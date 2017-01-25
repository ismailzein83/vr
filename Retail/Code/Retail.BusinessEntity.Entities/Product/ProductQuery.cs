using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductQuery
    {
        public string Name { get; set; }

        public int? ProductFamilyId { get; set; }
    }
}
