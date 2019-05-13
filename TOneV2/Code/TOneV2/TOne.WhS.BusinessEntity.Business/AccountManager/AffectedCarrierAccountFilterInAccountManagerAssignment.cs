using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AffectedCarrierAccountFilterInAccountManagerAssignment : ICarrierAccountFilter
    {
        public int? AccountManagerAssignmentId;
        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            var carrierAccount = context.CarrierAccount;

            if (context.CustomObject == null)
                context.CustomObject = new AccountManagerAssignmentCustomObject();
            var customObject = context.CustomObject as AccountManagerAssignmentCustomObject;

            var affectedCarrierAccountIds = customObject.AffectedCarrierAccountIds;
            if (customObject.AffectedCarrierAccountIds == null || !customObject.AffectedCarrierAccountIds.Contains(carrierAccount.CarrierAccountId))
                return false;

            else
            {
                var accountManagersAssignmet = new AccountManagerAssignmentManager().GetAllAccountManagersAssignmentByCarrierAccountId(carrierAccount.CarrierAccountId);

                foreach(var acctManagerAssgn in accountManagersAssignmet)
                {
                    if (AccountManagerAssignmentId != null && acctManagerAssgn.AccountManagerAssignmentId == AccountManagerAssignmentId)
                        return false;

                    switch (carrierAccount.AccountType)
                    {
                        case CarrierAccountType.Customer:
                            if (acctManagerAssgn.CustomerAssigned && !acctManagerAssgn.EED.HasValue)
                                return true;
                            break;

                        case CarrierAccountType.Supplier:
                            if(acctManagerAssgn.SupplierAssigned && !acctManagerAssgn.EED.HasValue)
                                return true;
                            break;

                        case CarrierAccountType.Exchange:
                            if(acctManagerAssgn.CustomerAssigned && acctManagerAssgn.SupplierAssigned && !acctManagerAssgn.EED.HasValue)
                                return true;
                            if(acctManagerAssgn.CustomerAssigned && !acctManagerAssgn.SupplierAssigned && !acctManagerAssgn.EED.HasValue)
                            {
                                var oppositeAcctManagerAssgn = accountManagersAssignmet.Where(itm => itm.CustomerAssigned == false && itm.SupplierAssigned == true && !itm.EED.HasValue);
                                if (oppositeAcctManagerAssgn != null)
                                    return true;
                            }
                            if (acctManagerAssgn.SupplierAssigned && !acctManagerAssgn.CustomerAssigned && !acctManagerAssgn.EED.HasValue)
                            {
                                var oppositeAcctManagerAssgn = accountManagersAssignmet.Where(itm => itm.SupplierAssigned == false && itm.CustomerAssigned == true && !itm.EED.HasValue);
                                if (oppositeAcctManagerAssgn != null)
                                    return true;
                            }
                            break;
                    }
                }
                return false;
            }
        }

        private class AccountManagerAssignmentCustomObject
        {
            public IEnumerable<int> AffectedCarrierAccountIds { get; set; }
            public AccountManagerAssignmentCustomObject()
            {
                AffectedCarrierAccountIds = new AccountManagerAssignmentManager().GetAffectedCarrierAccountIds();
            }
        }
    }
}
