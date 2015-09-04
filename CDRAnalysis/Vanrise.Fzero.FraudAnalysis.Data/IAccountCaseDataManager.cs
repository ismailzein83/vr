using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountCaseDataManager : IDataManager
    {
        AccountCase1 GetLastAccountCaseByAccountNumber(string accountNumber);
        bool InsertAccountCase(AccountCase1 accountCase, out int insertedID);
        bool UpdateAccountCase(AccountCase1 accountCase);
    }
}
