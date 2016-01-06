using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DWTimeManager
    {

        public List<Time> GetTimes(DateTime from, DateTime to)
        {
            IDWTimeDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWTimeDataManager>();

            return dataManager.GetTimes(from, to);
        }

    }
}
