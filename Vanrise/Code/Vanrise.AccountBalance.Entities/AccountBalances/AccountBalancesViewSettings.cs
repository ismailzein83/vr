using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalancesViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public Guid AccountTypeId { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_AccountBalance/Views/AccountBalances/AccountBalancesManagement/{{\"accountTypeId\":\"{0}\"}}", this.AccountTypeId);
        }
    }
}
