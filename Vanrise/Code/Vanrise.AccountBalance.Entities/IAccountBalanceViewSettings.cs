using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public interface IAccountBalanceViewSettings
    {
        List<Guid> GetAccountTypeIds();
    }
}
