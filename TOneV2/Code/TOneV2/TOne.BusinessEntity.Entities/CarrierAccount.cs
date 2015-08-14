using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierAccount
    {
        public string CarrierAccountId { get; set; }
        public Int16 ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileCompanyName { get; set; }
        public byte ActivationStatus { get; set; }
        public byte RoutingStatus { get; set; }
        public byte AccountType { get; set; }
        public byte CustomerPaymentType { get; set; }
        public byte SupplierPaymentType { get; set; }
        public string NameSuffix { get; set; }
        public int? CarrierGroupID { get; set; }
        public string CarrierGroupName { get; set; }
        public string CarrierGroups { get; set; }
        public List<int> GroupIds { get; set; }

        public string CarrierAccountName { get; set; }

        public int CarrierMaskId { get; set; }
    }
}
