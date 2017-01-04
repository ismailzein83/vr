using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBalanceManager
    {
        public Guid GetAccountBalanceTypeId(Guid accountBEDefinitionId)
        {
            Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            IEnumerable<AccountType> accountTypes = _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>();

            foreach(var accountType in accountTypes)
            {
                var extendedSettings =accountType.Settings.ExtendedSettings as SubscriberAccountBalanceSetting;
                if (extendedSettings != null && extendedSettings.AccountBEDefinitionId == accountBEDefinitionId)
                    return accountType.VRComponentTypeId;
            }
            return Guid.Empty;
        }
    }
}
