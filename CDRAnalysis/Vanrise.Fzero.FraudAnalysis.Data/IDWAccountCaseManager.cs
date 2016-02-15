using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWAccountCaseManager:IDataManager
    {
        List<DWAccountCase> GetDWAccountCases(DateTime from, DateTime to);
    }
}
