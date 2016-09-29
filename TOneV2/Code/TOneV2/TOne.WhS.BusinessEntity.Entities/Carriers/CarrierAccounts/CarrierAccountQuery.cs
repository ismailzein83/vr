using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountQuery
    {
        public List<int> CarrierAccountsIds { get; set; }
        public List<int> CarrierProfilesIds { get; set; }
        public string Name { get; set; }
        public List<CarrierAccountType> AccountsTypes { get; set; }
        public List<int?> SellingNumberPlanIds { get; set; }
        public List<int> Services { get; set; }
        public List<int> ActivationStatusIds { get; set; }
    }
}
