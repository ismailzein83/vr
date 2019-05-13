using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AccountManagerAssignment
    {
        public int AccountManagerAssignmentId { get; set; }
        public int AccountManagerId { get; set; }
        public int CarrierAccountId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
    }
}
