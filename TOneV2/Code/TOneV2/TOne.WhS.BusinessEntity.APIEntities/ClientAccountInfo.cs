using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.APIEntities
{
    public enum ClientAccountType { Exchange = 1, Supplier = 2, Customer = 3 }
    public enum ClientActivationStatus { Active = 0, Inactive=1, Testing = 2}
    public class ClientAccountInfo
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public ClientAccountType CarrierAccountType { get; set; }
        public ClientActivationStatus ActivationStatus { get; set; }
    }
}
