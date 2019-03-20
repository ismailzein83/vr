using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class SMSServiceType
    {
        public int SMSServiceTypeId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int CreatedBy { get; set; }

        public int LastModifiedBy { get; set; }
    }
}
