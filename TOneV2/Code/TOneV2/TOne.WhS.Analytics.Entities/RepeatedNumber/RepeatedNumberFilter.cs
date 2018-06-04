using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class RepeatedNumberFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<int> SupplierIds { get; set; }
    }
}