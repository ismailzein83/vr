using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountStatusHistoryDataManager:IDataManager
    {
        void Insert(Guid accountDefinitionId, long accountId, Guid statusDefinitionId);
    }
}
