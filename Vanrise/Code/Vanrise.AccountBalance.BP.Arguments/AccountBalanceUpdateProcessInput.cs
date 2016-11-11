using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.BP.Arguments
{
    public class AccountBalanceUpdateProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid AccountTypeId { get; set; }
        public override string GetTitle()
        {
            return "Account Balance Update";
        }
    }
}
