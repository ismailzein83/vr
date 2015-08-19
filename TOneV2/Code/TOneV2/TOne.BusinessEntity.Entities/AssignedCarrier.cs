using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class AssignedCarrier
    {
        public int UserId { get; set; }

        public string CarrierAccountId { get; set; }

        public string CarrierName { get; set; }

        public CarrierType RelationType { get; set; }
    }

    //public enum CarrierRelationType
    //{
    //    IsCustomer = 0,
    //    IsSupplier = 2
    //}
}
