using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.BI.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
namespace TOne.BusinessEntity.Business
{
    public class CustomerBehavior:IDimensionBehavior
    {
        public List<object> GetFilteredValues()
        {
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Customer);
            List<object> cutomers = new List<object>();
            foreach (AssignedCarrier assignedCarrier in assignedCarriers)
            {
                cutomers.Add(assignedCarrier.CarrierAccountId);
            }
            return cutomers;
        }
    }
}
