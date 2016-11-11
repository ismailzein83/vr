using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.AccountBalance
{
    public class AccountTypeManager
    {
        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
        public Guid GetAccountBEDefinitionId(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.AccountBusinessEntityDefinitionId;
        }

        public IEnumerable<BalanceAccountTypeInfo> GetAccountTypeInfo()
        {
            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings>().MapRecords(AccountTypeInfoMapper);
        }

        private BalanceAccountTypeInfo AccountTypeInfoMapper(VRComponentType<AccountTypeSettings> componentType)
        {
            return new BalanceAccountTypeInfo
            {
                Id = componentType.VRComponentTypeId,
                Name = componentType.Name
            };
        }

        AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<AccountTypeSettings>(accountTypeId);
        }
        BalanceAccountTypeInfo AccountTypeInfoMapper(AccountType accountType)
        {
            return new BalanceAccountTypeInfo
            {
                Id = accountType.VRComponentTypeId,
                Name = accountType.Name
            };
        }
    }
}
