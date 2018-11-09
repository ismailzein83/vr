using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.WhS.Entities
{
    public enum AccountViewType { Customer=0, Supplier=1}
    public class ClientRepeatedNumberFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<int> SupplierIds { get; set; }
        public List<string> ColumnsToShow { get; set; }
        public AccountViewType AccountType { get; set; }
    }
}
