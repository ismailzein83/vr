using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class ConfigurationManager
    {
        public int GetAccountBEDefinitionId()
        {
            return -2001;
        }

        public int GetBalanceAlertRuleDefinitionId()
        {
            throw new NotImplementedException();
        }
    }
}
