using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    //public class AffectedCarrierAccountFilterInAccountManagerAssignment : ICarrierAccountFilter
    //{
    //    //public int? AccountManagerAssignmentId;
    //    //public bool IsExcluded(ICarrierAccountFilterContext context)
    //    //{
    //    //    var carrierAccount = context.CarrierAccount;

    //    //    if (context.CustomObject == null)
    //    //        context.CustomObject = new AccountManagerAssignmentCustomObject();
    //    //    var customObject = context.CustomObject as AccountManagerAssignmentCustomObject;

    //    //    var affectedCarrierAccountIds = customObject.AffectedCarrierAccountIds;
    //    //    if (customObject.AffectedCarrierAccountIds == null || !customObject.AffectedCarrierAccountIds.Contains(carrierAccount.CarrierAccountId))
    //    //        return false;

    //    //    else
    //    //    {
    //    //        var accountManagerAssignmet = new AccountManagerAssignmentManager().GetAccountManagerAssignmentByCarrierAccountId(carrierAccount.CarrierAccountId);

    //    //        if (AccountManagerAssignmentId != null  && accountManagerAssignmet.AccountManagerAssignmentId == AccountManagerAssignmentId)
    //    //            return false;

    //    //        if (carrierAccount.AccountType == CarrierAccountType.Customer && accountManagerAssignmet.CustomerAssigned && !accountManagerAssignmet.EED.HasValue)
    //    //            return true;

    //    //        if (carrierAccount.AccountType == CarrierAccountType.Supplier && accountManagerAssignmet.SupplierAssigned && !accountManagerAssignmet.EED.HasValue)
    //    //            return true;

    //    //        if (carrierAccount.AccountType == CarrierAccountType.Exchange && accountManagerAssignmet.CustomerAssigned && accountManagerAssignmet.SupplierAssigned && !accountManagerAssignmet.EED.HasValue)
    //    //            return true;

    //    //        return false;
    //    //    }
    //    //}

    //    //private class AccountManagerAssignmentCustomObject
    //    //{
    //    //    public IEnumerable<int> AffectedCarrierAccountIds { get; set; }
    //    //    public AccountManagerAssignmentCustomObject()
    //    //    {
    //    //        AffectedCarrierAccountIds = new AccountManagerAssignmentManager().GetAffectedCarrierAccountIds();
    //    //    }
    //    //}
    //}
}
