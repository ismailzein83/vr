using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.BP.Arguments
{
    public class AccountBalanceUpdateProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid AccountTypeId { get; set; }
        public int UsageCacheDays { get; set; }
        public override string GetTitle()
        {
            return $"Account Balance Updater '{BusinessManagerFactory.GetManager<IAccountTypeManager>().GetAccountTypeName(this.AccountTypeId)}'";
        }
    }
}
