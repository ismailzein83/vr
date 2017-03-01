using System.Collections.Generic;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountTypeFilter
    {
        public List<IAccountTypeExtendedSettingsFilter> Filters { get; set; }
    }
}
