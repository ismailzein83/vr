using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;


namespace Demo.Module.Data
{
    public interface IOperatorAccountDataManager:IDataManager
    {
        List<OperatorAccount> GetOperatorAccounts();
        bool Insert(OperatorAccount operatorAccount,out int operatorAccountId);
        bool Update(OperatorAccount operatorAccount);
        bool AreOperatorAccountsUpdated(ref object updateHandle);
    }
}
