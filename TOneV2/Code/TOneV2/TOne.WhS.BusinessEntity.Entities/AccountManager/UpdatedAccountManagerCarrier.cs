using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class UpdatedAccountManagerCarrier
    {
        public int UserId { get; set; }
        public string CarrierAccountId { get; set; }
        public int RelationType { get; set; }
        public bool Status { get; set; }
    }
}
