using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IAccountBEManager : IBEManager
    {
        string GetAccountName(Guid accountBEDefinitionId, long accountId);
    }
}
