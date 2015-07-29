using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.BI.Entities;
using Vanrise.Security.Business;

namespace TOne.BusinessEntity.Business
{
    public class SupplierBehavior : IDimensionBehavior
    {
        public List<string> GetFilteredValues()
        {
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierTypeFilter.OnlySuppliers);
            List<string> suppliers = new List<string>();
            foreach (AssignedCarrier assignedCarrier in assignedCarriers)
            {
                suppliers.Add(assignedCarrier.CarrierAccountId);
            }
            return suppliers;
        }
    }
}
