using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Data
{
    public interface IOperatorAccountDataManager : IDataManager
    {
        List<OperatorAccount> GetOperatorAccounts();
    }
}
