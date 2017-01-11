using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionViewSettings : Vanrise.Security.Entities.ViewSettings, IAccountBalanceViewSettings
    {
        public List<BillingTransactionViewItem> Items { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_AccountBalance/Views/BillingTransaction/BillingTransactionManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
        public List<Guid> GetAccountTypeIds()
        {
            return  this.Items.Select(x=>x.AccountTypeId).ToList();
        }
    }

    public class BillingTransactionViewItem
    {
        public Guid AccountTypeId { get; set; }
    }
}
