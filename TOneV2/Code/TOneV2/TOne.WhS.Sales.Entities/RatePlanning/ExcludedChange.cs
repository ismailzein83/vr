using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ExcludedChange
    {
        public int SubscriberId { get; set; }
        public List<int> CountryIds { get; set; }
        public string Reason { get; set; }
    }
}
