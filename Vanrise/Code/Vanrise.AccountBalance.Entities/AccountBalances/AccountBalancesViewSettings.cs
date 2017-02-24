using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalancesViewSettings : Vanrise.Security.Entities.ViewSettings, IAccountBalanceViewSettings
    {
        public List<AccountBalanceViewItem> Items { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_AccountBalance/Views/AccountBalances/AccountBalancesManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public List<Guid> GetAccountTypeIds()
        {
            return  this.Items.Select(x=>x.AccountTypeId).ToList();
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            if (this.GetAccountTypeIds() != null)
                return BusinessManagerFactory.GetManager<IAccountTypeManager>().DoesUserHaveViewAccess(context.UserId, this.GetAccountTypeIds());
            else
                return false;

        }
    }

    public class AccountBalanceViewItem
    {
        public Guid AccountTypeId { get; set; }
    }

}
