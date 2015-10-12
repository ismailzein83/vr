using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerGroupSettings
    {
        public int ConfigId { get; set; }
    }

    public class SelectiveCustomersSettings : CustomerGroupSettings
    {
        public List<int> CustomerIds { get; set; }
    }
}
