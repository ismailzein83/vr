using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CustomerInfo
    {
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public string Gender{ get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
