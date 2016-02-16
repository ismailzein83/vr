using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum OperatorAccountType { Exchange = 1, Supplier = 2, Customer = 3}

    public class OperatorAccount
    {
        public int OperatorAccountId { get; set; }
        public string NameSuffix { get; set; }
        public int OperatorProfileId { get; set; }
        public OperatorAccountSettings OperatorAccountSettings { get; set; } 
        public OperatorAccountSupplierSettings SupplierSettings { get; set; }
        public OperatorAccountCustomerSettings CustomerSettings { get; set; }
    }
}
