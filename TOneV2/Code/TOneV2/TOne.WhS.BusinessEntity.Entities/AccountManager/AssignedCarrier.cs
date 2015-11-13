using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AssignedCarrier
    {
        public int UserId { get; set; }

        public string CarrierAccountId { get; set; }

        public string CarrierName { get; set; }

        public CarrierAccountType RelationType { get; set; }
    }
}
