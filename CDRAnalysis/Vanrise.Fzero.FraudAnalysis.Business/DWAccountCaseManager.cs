using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DWAccountCaseManager
    {
        public List<DWAccountCase> GetDWAccountCases(DateTime from, DateTime to)
        {
            IDWAccountCaseManager dataManager = FraudDataManagerFactory.GetDataManager<IDWAccountCaseManager>();
            return dataManager.GetDWAccountCases(from, to);
        }
    }
}
