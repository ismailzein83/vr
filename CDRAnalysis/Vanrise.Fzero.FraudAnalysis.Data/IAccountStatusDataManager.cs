using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountStatusDataManager : IDataManager 
    {
        List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, IEnumerable<string> numberPrefixes);
    }
}
