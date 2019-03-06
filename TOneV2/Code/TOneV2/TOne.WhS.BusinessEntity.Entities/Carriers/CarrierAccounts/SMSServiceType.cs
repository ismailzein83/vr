using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SMSServiceType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int CreatedBy { get; set; }

        public int LastModifiedBy { get; set; }
    }
}
