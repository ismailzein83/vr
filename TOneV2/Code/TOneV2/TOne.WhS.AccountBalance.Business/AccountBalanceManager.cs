using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceManager
    {
        public void ParseEntityId(string balanceEntityId, out int accountOrProfileId, out bool isProfileId)
        {
            throw new NotImplementedException();
        }

        public Decimal GetCustomerCreditLimit(string balanceEntityId)
        {
            throw new NotImplementedException();
        }

        public decimal GetSupplierCreditLimit(string balanceEntityId)
        {
            throw new NotImplementedException();
        }

        public decimal GetCustomerEstimatedMaxBalance(string balanceEntityId)
        {
            throw new NotImplementedException();
        }

        public decimal GetSupplierEstimatedMaxBalance(string balanceEntityId)
        {
            throw new NotImplementedException();
        }
    }
}
