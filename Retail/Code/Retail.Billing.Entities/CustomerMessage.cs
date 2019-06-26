using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class CustomerMessage
    {
        public string Message { get; set; }
        public IEnumerable<CustomerMessage> GetCustomerMessagesRDLCSchema()
        {
            return null;
        }
    }
}
