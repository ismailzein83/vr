using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerType
    {
        public Guid DealerTypeId { get; set; }

        public string Name { get; set; }

        public DealerTypeSettings Settings { get; set; }
    }
}
